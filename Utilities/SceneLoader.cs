using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        HomeScene,
        LoadingScene,
        MainMenuScene,
        GameScene,
        TutorialScene,
        LobbyScene,
        DeckManagerScene,
        CardCatalogueScene,
        DeckEditorScene,
        CharacterSelectScene
    }
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        SceneLoader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());


    }
    public static void LoadNetwork(Scene targetScene)
    {
        Debug.LogWarning("@@@@@@@@@@@@@ALLoadNetworkTED");
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}


