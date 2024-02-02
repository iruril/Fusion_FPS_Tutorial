using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;
using System;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    private NetworkRunner _myNetworkRunner = null;

    void Start()
    {
        _myNetworkRunner = Instantiate(networkRunnerPrefab, transform.position, Quaternion.identity);
        _myNetworkRunner.name = "Network runner";

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var clientTask = InitializeNetworkRunner(_myNetworkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), scene, null);
        Debug.Log($"[서버] 네트워크 러너가 시작되었습니다.");
    }

    protected virtual async Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef sceneRef, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if(sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = sceneRef,
            SessionName = "TestRoom",
            OnGameStarted = initialized,
            SceneManager = sceneManager
        });
    }
}
