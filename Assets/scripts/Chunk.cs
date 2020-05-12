using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

	public const int ChunkStack = 10;
	public const int width = 5, Height = 5;



	public static Dictionary<Vector3, Chunk> Chunks = new Dictionary<Vector3, Chunk>();

	public List<byte[,,]> maps = new List<byte[,,]>();
	public byte[,] heightMap = new byte[width, width];
	public List<MeshFilter> meshes = new List<MeshFilter>();

	public List<List<Vector3>> verts = new List<List<Vector3>>();
	public List<List<int>> triangles = new List<List<int>>();
	public List<List<Vector2>> uvs = new List<List<Vector2>>();
	public List<List<Color>> colors = new List<List<Color>>();


	public bool dirty = true;
	public bool lightDirty = true;
	public bool calculatedMap = false;
	public static bool working = false;


	void Awake()
	{
		transform.parent = World.me;
		for (int i = 0; i < ChunkStack; i++)
		{
			maps.Add(new Byte[width, Height, width]);
			GameObject go = new GameObject("ChunkStacck" + i);

			meshes.Add(go.AddComponent<MeshFilter>());

			MeshRenderer me = go.AddComponent<MeshRenderer>();


			go.transform.position = new Vector3(transform.position.x, i * Height, transform.position.z);
			go.transform.parent = transform;

			me.material = World.mats;
		}
		heightMap = new Byte[width, width];
	}

	public bool isBlockTransparent(int x,int y,int z)
	{
		if(x>=width || z>=width || y>=Height || x<0 || y<0|| z < 0)
		{

		}
		else
		{
			int chunkID = Height / y;
			return (maps[chunkID][x, y, z] == 0);
		}
	}
	public byte GetBack(Vector3 worldPos)
	{

	}
	void Update()
	{
		if (dirty)
		{
			working = true;
			if (!calculatedMap)
			{
				CMap();
			}
			Cmash();
			dirty = false;
		}
		if (lightDirty)
		{
			working = true;
			Clight();
			lightDirty = false;
		}
	}

	public void CMap()
	{
		working = false;
		for (int i = 0; i < ChunkStack; i++)
		{
			byte[,,] map = new byte[width, Height, width];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					for (int z = 0; z < width; z++)
					{
						int worldYPos = (i * Height) + y;
						if (worldYPos < 5)
							map[x, y, z] = 1;
					}
				}
			}
			maps[i] = map;
		}
	}
	public void Cmash()
	{
		verts.Clear();
		triangles.Clear();
		uvs.Clear();
		colors.Clear();

		for (int i = 0; i < ChunkStack; i++)
		{
			byte[,,] map = maps[i];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					for (int z = 0; z < width; z++)
					{
						if (map[x, y, z] > 0)
						{
							AddFace(x, y, z, FaceDir.Top, i);
							AddFace(x, y, z, FaceDir.Down, i);
							AddFace(x, y, z, FaceDir.Right, i);
							AddFace(x, y, z, FaceDir.Left, i);
							AddFace(x, y, z, FaceDir.Back, i);
							AddFace(x, y, z, FaceDir.Front, i);
							
						}
					}
				}
			}
			if (verts.Count <= i) continue;
			Mesh m = new Mesh();
			m.vertices = verts[i].ToArray();
			m.triangles = triangles[i].ToArray();
			m.uv = uvs[i].ToArray();
			m.RecalculateNormals();
			meshes[i].mesh = m;
		}
		working = false;
	}
	public void Clight()
	{
		working = false;
	}

	public void AddFace(int x, int y, int z, FaceDir dir, int chunkID)
	{
		if (verts.Count >= chunkID)
		{
			verts.Add(new List<Vector3>());
		}
		if (triangles.Count >= chunkID)
		{
			triangles.Add(new List<int>());
		}
		if (uvs.Count >= chunkID)
		{
			uvs.Add(new List<Vector2>());
		}
		if (colors.Count >= chunkID)
		{
			colors.Add(new List<Color>());
		}
		switch (dir)
		{
			case FaceDir.Top:
				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 1);

				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 2);

				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 1));
				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 1));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));


				break;
			case FaceDir.Down:
				triangles[chunkID].Add(verts[chunkID].Count + 1);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 1));
				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 1));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));


				break;
			case FaceDir.Front:
				triangles[chunkID].Add(verts[chunkID].Count + 1);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				verts[chunkID].Add(new Vector3(x + 0, y + 0, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 1));
				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 1));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));
				break;
			case FaceDir.Right:
				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 1);

				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 2);

				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 0));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));
				break;
			case FaceDir.Left:
				triangles[chunkID].Add(verts[chunkID].Count + 1);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 0);

				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 1));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 0));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));
				break;
			case FaceDir.Back:
				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 2);
				triangles[chunkID].Add(verts[chunkID].Count + 1);

				triangles[chunkID].Add(verts[chunkID].Count + 0);
				triangles[chunkID].Add(verts[chunkID].Count + 3);
				triangles[chunkID].Add(verts[chunkID].Count + 2);

				verts[chunkID].Add(new Vector3(x + 0, y + 0, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 0, z + 0));
				verts[chunkID].Add(new Vector3(x + 1, y + 1, z + 0));
				verts[chunkID].Add(new Vector3(x + 0, y + 1, z + 0));

				uvs[chunkID].Add(new Vector2(0, 0));
				uvs[chunkID].Add(new Vector2(1, 0));
				uvs[chunkID].Add(new Vector2(1, 1));
				uvs[chunkID].Add(new Vector2(0, 1));
				break;
			default:
				break;

		}
		if (dir == FaceDir.Top)
		{
			
		}

	}

	public static Chunk GetChunk(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x / width) * width;
		int y = Mathf.FloorToInt(pos.y / width) * width;
		int z = Mathf.FloorToInt(pos.z / width) * width;
		Vector3 cPos = new Vector3(x, y, z);
		if (Chunks.ContainsKey(cPos))
		{
			return Chunks[cPos];
		}

		return null;
	}
	public static bool ChunkExists(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x / width) * width;
		int y = Mathf.FloorToInt(pos.y / width) * width;
		int z = Mathf.FloorToInt(pos.z / width) * width;
		Vector3 cPos = new Vector3(x, y, z);
		Console.WriteLine("chnk exis");
		if (Chunks.ContainsKey(cPos))
		{
			return true;
		}

		return false;
	}

	public static bool AddChunk(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x / width) * width;
		int y = Mathf.FloorToInt(pos.y / width) * width;
		int z = Mathf.FloorToInt(pos.z / width) * width;
		Vector3 cPos = new Vector3(x, y, z);
		Debug.Log(cPos);
		if (Chunks.ContainsKey(cPos))
		{
			return false;
		}

		GameObject chunk = new GameObject("Chunk" + cPos);
		Chunk c = chunk.AddComponent<Chunk>();
		MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
		chunk.AddComponent<MeshFilter>();

		chunk.transform.position = cPos;
		mr.material = World.mats;

		Chunks.Add(cPos, c);
		return true;
	}
	public static bool RemoveChunk(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x / width) * width;
		int y = Mathf.FloorToInt(pos.y / width) * width;
		int z = Mathf.FloorToInt(pos.z / width) * width;
		Vector3 cPos = new Vector3(x, y, z);
		Debug.Log(cPos);
		if (Chunks.ContainsKey(cPos))
		{
			Destroy(Chunks[cPos].gameObject);
			Chunks.Remove(cPos);

			return true;
		}

		return false;
	}
}
public enum FaceDir
{
	Top,
	Down,
	Right,
	Left,
	Front,
	Back
}
