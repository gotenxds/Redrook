using UnityEngine;
using UnityEngine.SceneManagement;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;

public class DataController : MonoBehaviour
{
    private Ailment[] ailments;
    private Element[] elements;
    void Awake()
    {
        SaveGame.Serializer = new SaveGameBinarySerializer();

        DontDestroyOnLoad(gameObject);

        ailments = SaveGame.Load<Ailment[]>("Ailments");
        elements = SaveGame.Load<Element[]>("Elements");
    }

    public Ailment[] GetAilments()
    {
        return ailments;
    }

    public Element[] GetElements()
    {
        return elements;
    }
}