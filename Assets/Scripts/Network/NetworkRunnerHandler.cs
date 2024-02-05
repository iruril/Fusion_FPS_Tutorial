using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    private NetworkRunner _myNetworkRunner = null;

    void Start()
    {
        _myNetworkRunner = Instantiate(networkRunnerPrefab, transform.position, Quaternion.identity);
        _myNetworkRunner.name = "Network runner";

        var clientTask = InitializeNetworkRunner(
            _myNetworkRunner,
            GameMode.AutoHostOrClient,
            GameManager.Instance.GetConnectionToken(),
            NetAddress.Any(),
            SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            null);
        Debug.Log($"[Server] NetworkRunner Started.");
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        _myNetworkRunner = Instantiate(networkRunnerPrefab, transform.position, Quaternion.identity);
        _myNetworkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(_myNetworkRunner, hostMigrationToken);
        Debug.Log($"[Server] NetworkRunner - Host Migration Started.");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if(sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, byte[] connectionToken, NetAddress netAddress, SceneRef sceneRef, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = "TestRoom",
            OnGameStarted = initialized,
            SceneManager = sceneManager,
            ConnectionToken = connectionToken
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = HostMigrationResume,
            ConnectionToken = GameManager.Instance.GetConnectionToken()
        });
    }

    private void HostMigrationResume(NetworkRunner runner)
    {
        foreach(var item in runner.GetResumeSnapshotNetworkObjects())
        {
            if(item.TryGetBehaviour<NetworkCharacterController>(out var kcc))
            {
                runner.Spawn(item, kcc.Data.Position, kcc.Data.Rotation, onBeforeSpawned: (runner, newNetworkObject) =>
                {
                    newNetworkObject.CopyStateFrom(item);

                    if (item.TryGetBehaviour<HPHandler>(out var oldHP))
                    {
                        HPHandler newHP = newNetworkObject.GetComponent<HPHandler>();
                        newHP.CopyStateFrom(oldHP);
                        newHP.skipSettingStartingValue = true;
                    }

                    if (item.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                    {
                        FindObjectOfType<PlayerSpawner>().SetConnectionTokenMapping
                        (
                            oldNetworkPlayer.token,
                            newNetworkObject.GetComponent<NetworkPlayer>()
                        );
                    }
                });
            }
        }

        StartCoroutine(CleanUpHostMigration());
    }

    private IEnumerator CleanUpHostMigration()
    {
        yield return new WaitForSeconds(5.0f);
        FindAnyObjectByType<PlayerSpawner>().OnHostMigrationCleanUP();
    }
}
