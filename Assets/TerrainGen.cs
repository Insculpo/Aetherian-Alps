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
	//List<Vector2> treeList;
	// Use this for initialization
	void Start () {
		//biomeMap.Resize (AAlps.detailWidth, AAlps.detailHeight);
	//	treeList = new Vector2<float>();
		AAlps.size = new Vector3(width, depth, width);
		heightField = new float[width,width];
		GenerateTerrain (biomeMap,AAlps);
		GenerateBiomeTexture (biomeMap,AAlps);
		PlaceTreesAcrossTerrain (100, biomeMap);
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
					heightField [y, x] = CalculateHeight (y, x, 100.0f, 7) * scale;
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
        for (int k = 0; k < oct; k++)
        {
            float xCoord = offsetX + (float)x / width * frequency;
            float yCoord = offsetY + (float)y / width * frequency;
            uVal += Mathf.PerlinNoise(xCoord, yCoord) * biome * frequency;

            max += amp;
            biome *= persistance;
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
				float abAngle = aM.GetSteepness (normX, normY);
				float tShift = abAngle / 90f;

				float[] splatW = new float[aM.alphamapLayers];

				Vector3 norm = aM.GetInterpolatedNormal (x, y);

				if (bM.GetPixel (x, y).g > 0.5) {
					float zv = -0.5f;
					splatW [2] = zv + (tShift * 2);
				}
						//Normalizes 
						splatW [0] = 1 - (bM.GetPixel(x,y).g * 0.8f);
						splatW [1] = bM.GetPixel(x,y).g ;

				float z = splatW.Length;
				for (int i = 0; i < splatW.Length; i++) {
					splatW [i] /= z;
					newData [y, x, i] = splatW [i];
				}
				



			}
		}
		aM.SetAlphamaps (0, 0, newData);
	}



	public void PlaceTreesAcrossTerrain(int numTrees, Texture2D bM){
		float treeX = 0;
		float treeZ = 0;
		float trueTree = numTrees;
		for (int i = 0; i < trueTree; i++) {
			if (bM.GetPixel(treeX, treeZ).g < 0.5) {
				treeX = Random.Range (0, 1f);
				treeZ = Random.Range (0, 1f);
				PlaceTree (treeX, treeZ);
				trueTree--;
			} 
		}
	}

	void PlaceTree(float tX, float tY) {
		//	TreeInstance newTree;
		int numPrototypes = aM.terrainData.treePrototypes.Length;
		float tAngle = aM.terrainData.GetSteepness (tX, tY);
		float tFrac = tAngle / 90.0f;
		//int t = Random.Range (numPrototypes-2, numPrototypes);
		if (numPrototypes == 0) {
			Debug.Log ("No trees!");
			return;
		}
		//Propability based placement
		int p = Random.Range (0, 99);

		int b = Random.Range (0.5, 1.5);
		//Occassional MEGA trees!
		if (p > 90) {
			b = Random.Range (4.5, 5.5);
		}
		float r = Random.Range (0f, 6.238f);
		if (tFrac < 0.32f && b < 4) {
			Vector3 pos = new Vector3 (tX, 0, tY);
			TreeInstance newTree = new TreeInstance ();
			newTree.position = pos;
			newTree.color = GetTreeColor ();
			newTree.lightmapColor = Color.white;
			newTree.prototypeIndex = 0;
			newTree.heightScale = b;
			newTree.widthScale =  b/2;
			newTree.rotation = r;
			terrain.AddTreeInstance (newTree);
			treeField.Add (newTree);
		}

	}

	private Color GetTreeColor() {
		Color color = Color.white * Random.Range (1f, 1f - treeColorAdjustment);
		color.a = 1f;
		return color;
	}


	public void newOctave(float o){
		octaves = Mathf.RoundToInt(o);		
	}
	public void newHeight(float h){
		depth = Mathf.RoundToInt(h);	
	}
	public void newPersistance(float p){
		persistance = p;	
	}
	public void newscale(float s){
		scale = s;	
	}

/*	void killTree(float x, float y)
	{
		for(i =
			if(x == treeList[i].)
			{
			}
	}*/

	float GetSum(float[] g) {
		float mean = 0f;
		for (int i = 0; i < g.Length; i++) {
			g [i] += mean;
		}
		return mean / g.Length;
	}
}
