

using UnityEngine;

public class World : MonoBehaviour
{

    public Material mat;
    public static Material mats;
    public static Transform me;

    public const int viewRange = 10;

    void Awake()
    {
        mats = mat;
        me = transform;
    }
    void Update()

    {
        for (int x = 0; x < (viewRange / 2); x++)
        {
            for (int z = 0; z < (viewRange / 2); z++)
            {
                Vector3 cPos = new Vector3(x * Chunk.width, 0, z * Chunk.width);
                if (!Chunk.ChunkExists(cPos))
                {
                    Debug.Log("hello");
                    Chunk.AddChunk(cPos);
                }

            }
        }
    }
}


