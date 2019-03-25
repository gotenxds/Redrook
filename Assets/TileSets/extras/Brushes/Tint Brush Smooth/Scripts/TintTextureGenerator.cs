using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

[ExecuteInEditMode]
public class TintTextureGenerator : MonoBehaviour
{
	public int k_TintMapSize = 100;
	private const float toRedrookSpace = -1f;

	public void Start()
	{
		Refresh(GetComponent<Grid>());
	}

	private Texture2D m_TintTexture;
	private Texture2D tintTexture
	{
		get
		{
			if (m_TintTexture == null)
			{

				m_TintTexture = new Texture2D(k_TintMapSize, k_TintMapSize, TextureFormat.ARGB32, false)
				{
					hideFlags = HideFlags.HideAndDontSave,
					wrapMode = TextureWrapMode.Clamp,
					filterMode = FilterMode.Bilinear
				};		

				m_TintTexture.SetPixels(Enumerable.Repeat(Color.white, k_TintMapSize * k_TintMapSize).ToArray());
				
				RefreshGlobalShaderValues();
			}
			
			return m_TintTexture;
		}
	}

	public void Refresh(Grid grid)
	{
		if (grid == null)
			return;

		var gridPositionsProperties = GetGridInformation(grid).GetPositionProperties("Tint");
		
		for (var i = gridPositionsProperties.Count - 1; i >= 0; i--)
		{
			var positionsProperty = gridPositionsProperties[i];
			var texPos = WorldToTexture(positionsProperty.position);
			tintTexture.SetPixel(texPos.x, texPos.y, positionsProperty.Item2);
		}
			
		tintTexture.Apply();
	}

	public void Refresh(Grid grid, Vector3Int position)
	{
		if (grid == null)
			return;

		var scenePosition = SceneUtils.GetScenePosition(gameObject.scene);
		
		RefreshGlobalShaderValues();
		Vector3Int texPosition = WorldToTexture(position);
		tintTexture.SetPixel(texPosition.x, texPosition.y, GetGridInformation(grid).GetPositionProperty(position, "Tint", Color.white));
		tintTexture.Apply();
	}

	public Color GetColor(Grid grid, Vector3Int position)
	{
		if (grid == null)
			return Color.white;

		return GetGridInformation(grid).GetPositionProperty(position, "Tint", Color.white);
	}

	public void SetColor(Grid grid, Vector3Int position, Color color)
	{
		if (grid == null)
			return;

		GetGridInformation(grid).SetPositionProperty(position, "Tint", color);
		Refresh(grid, position);
	}

	private Vector3Int WorldToTexture(Vector3Int world)
	{
		return new Vector3Int(world.x + tintTexture.width / 2, world.y + tintTexture.height / 2, 0);
	}

	Vector3Int TextureToWorld(Vector3Int texpos)
	{
		return new Vector3Int(texpos.x - tintTexture.width / 2, texpos.y - tintTexture.height / 2, 0);
	}

	GridInformation GetGridInformation(Grid grid)
	{
		GridInformation gridInformation = grid.GetComponent<GridInformation>();

		if (gridInformation == null)
			gridInformation = grid.gameObject.AddComponent<GridInformation>();

		return gridInformation;
	}

	void RefreshGlobalShaderValues()
	{
		Shader.SetGlobalTexture("_TintMap", m_TintTexture);
		Shader.SetGlobalFloat("_TintMapSize", k_TintMapSize);
		Shader.SetGlobalVector("scenePos", SceneUtils.GetScenePosition(gameObject.scene) * toRedrookSpace);
	}

	public bool IsValidPosition(Vector3 position)
	{
		var halfSize = k_TintMapSize / 2;
		var min = halfSize - k_TintMapSize;
		var max = halfSize - 1;

		return IsInRange(position.x, min, max) &&
		       IsInRange(position.y, min, max);
	}

	private bool IsInRange(float num, float min, float max)
	{
		return num >= min && num <= max;
	}
}
