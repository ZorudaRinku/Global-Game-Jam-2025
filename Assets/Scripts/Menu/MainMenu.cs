using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public NumPlayers numPlayers;

    // game launchers, saves number of players
    public void Play2Player()
    {
        Debug.Log("Launching 2 Player Game");
        numPlayers.numberOfPlayers = 2;
        SceneManager.LoadScene("GameScene");
    } // Play2PLayer

    public void Play3Player()
    {
        Debug.Log("Launching 3 Player Game");
        numPlayers.numberOfPlayers = 3;
        SceneManager.LoadScene("GameScene");
    } // Play3Player

    public void Play4Player()
    {
        Debug.Log("Launching 4 Player Game");
        numPlayers.numberOfPlayers = 4;
        SceneManager.LoadScene("GameScene");
    } // Play4Player

    // Exit game
    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    } // Quit

} // MainMenu
