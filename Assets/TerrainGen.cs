using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour {

	public TerrainData AAlps;
	public Texture2D biomeMap;
	public int depth = 20;
	public int width = 256;
	public int height = 256;
	public int octaves = 4;
	public float scale = 20f;
	public int treeCount = 100;
	public float persistance = 1f;
	public float offsetX = 0f;
	public float offsetY = 0f;
	public float heightScalar = 1f;
	public float treeDensity;
	public float treeColorAdjustment = 0.4f;
	float[,] heightField;


	// Use this for initialization
	void Start () {
		//biomeMap.Resize (AAlps.detailWidth, AAlps.detailHeight);

		AAlps.size = new Vector3(width, depth, width);
		heightField = new float[width,width];
		GenerateTerrain (biomeMap,AAlps);
		GenerateBiomeTexture (biomeMap,AAlps);
	}

	// Update is called once per frame
	void Update () {

	}
		
	void GenerateTerrain (Texture2D bM, TerrainData aM)
	{
		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {
				if (bM.GetPixel (x, y).g < 0.5) {
					//Normalizes 
					heightField [y, x] = CalculateHeight (y, x, 1.0f, 1) * scale;
				} else {
					heightField [y, x] = CalculateHeight (y, x, 10.0f, 7) * scale;
				}
				//GameObject.Instantiate(trees,vigor,
			}
		}
		aM.SetHeights (0, 0, heightField);
	}

	float CalculateHeight(int x, int y,float biome, int oct)
	{

		float uVal = 0.0f;
		float max = 0.0f;
		float amp = 1.0f;
		float frequency = 1.0f;
		for (int k = 0; k < oct; k++) {
			float xCoord = offsetX + (float)x / width * frequency;
			float yCoord = offsetY + (float)y / width * frequency;
			uVal += Mathf.PerlinNoise (xCoord, yCoord) * biome;

			max += amp;
			amp *= persistance;
			frequency *= 2;
		}

		return uVal / max;

	}

	void GenerateBiomeTexture(Texture2D bM, TerrainData aM)
	{
		float [,,] newData = new float[aM.alphamapWidth, aM.alphamapHeight, aM.alphamapLayers];
		for (int y = 0; y < aM.alphamapHeight; y++) {
			for (int x = 0; x < aM.alphamapWidth; x++) {
				float normX = x * 1.0f / (aM.alphamapWidth - 1);
				float normY = y * 1.0f / (aM.alphamapHeight - 1);

				float[] splatW = new float[aM.alphamapLayers];

				Vector3 norm = aM.GetInterpolatedNormal (x, y);

				if(bM.GetPixel(x,y).g < 0.5) {
						//Normalizes 
						splatW [0] = 0.0f;
						splatW [1] = 1.0f;

				}
				else
				{
						//Normalizes 
					splatW [0] = 1.0f;
					splatW [1] = 0.0f;
				}
				float z = splatW.Length;
				for (int i = 0; i < splatW.Length; i++) {
					splatW [i] /= z;
					newData [y, x, i] = splatW [i];
				}
				



			}
		}
		aM.SetAlphamaps (0, 0, newData);
	}
}
