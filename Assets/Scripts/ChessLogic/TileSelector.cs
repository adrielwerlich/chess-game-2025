using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{

	public GameObject m_tileHighlightPrefab;
	public Board m_board;

	private GameObject m_lastHoveredPiece = null;

	private GameObject m_tileHighlight;
	private Renderer m_lastPieceRenderer = null;
	private Material m_originalMaterial = null;

	private Material m_selectedMaterial;
	private GameObject m_selectedPiece = null;
	private GameObject m_currentHoverPiece = null;
	private Dictionary<GameObject, Material> m_originalMaterials = new Dictionary<GameObject, Material>();
	private GameManager m_gameManager;

	void Start()
	{
		m_tileHighlightPrefab = Resources.Load<GameObject>("Prefabs/Selection-Yellow");
		m_gameManager = GameManager.instance;
		m_board = GetComponent<Board>();
		m_selectedMaterial = m_board.selectedMaterial;
		Vector2Int gridPoint = Geometry.GridPoint(0, 0);
		Vector3 point = Geometry.PointFromGrid(gridPoint);
		m_tileHighlight = Instantiate(m_tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
		m_tileHighlight.SetActive(false);
	}

	void OnEnable()
	{
		GameManager.instance.OnPiecesInitialized += CacheAllPieceMaterials;
	}

	void OnDisable()
	{
		if (GameManager.instance != null)
			GameManager.instance.OnPiecesInitialized -= CacheAllPieceMaterials;
	}

	private void CacheAllPieceMaterials()
	{
		m_originalMaterials.Clear();
		var pieces = GameManager.instance.pieces;
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				GameObject piece = pieces[x, y];
				if (piece != null)
				{
					Renderer renderer = piece.GetComponentInChildren<Renderer>();
					if (renderer != null && !m_originalMaterials.ContainsKey(piece))
					{
						m_originalMaterials[piece] = renderer.material;
					}
				}
			}
		}
	}

	void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			Vector3 point = hit.point;
			Vector2Int gridPoint = Geometry.GridFromPoint(point);

			m_tileHighlight.SetActive(true);
			m_tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
			if (Input.GetMouseButtonDown(0))
			{
				GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
				if (GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
				{
					GameManager.instance.SelectPiece(selectedPiece);
					// Reference Point 1: add ExitState call here later

					// CASE 3: When the hovered piece becomes the selected piece
					// Remove hover effect from last hovered piece
					RestoreLastHoveredPiece();


					ExitState(selectedPiece);
				}
			}


			if (hit.collider.CompareTag("Piece"))
			{
				GameObject hoveredPiece = hit.collider.gameObject;
				Renderer pieceRenderer = hoveredPiece.GetComponentInChildren<Renderer>();


				// Only apply hover effect if not the selected piece and not already hovered
				if (pieceRenderer != null && hoveredPiece != m_selectedPiece && hoveredPiece != m_lastHoveredPiece)
				{
					RestoreAllHoverEffects();
					// RestoreLastHoveredPiece(); // Remove effect from previous
					// Store original material if not already stored
					if (!m_originalMaterials.ContainsKey(hoveredPiece))
					{
						m_originalMaterials[hoveredPiece] = pieceRenderer.material;
					}
					// Apply hover effect
					Material randomMat = new Material(pieceRenderer.material);
					Color glowColor = new Color(Random.value, Random.value, Random.value);
					randomMat.color = glowColor;
					randomMat.EnableKeyword("_EMISSION");
					randomMat.SetColor("_EmissionColor", glowColor * Random.Range(0.1f, 0.55f));
					pieceRenderer.material = randomMat;
					m_lastPieceRenderer = pieceRenderer;
					m_lastHoveredPiece = hoveredPiece;
				}
			}
			else
			{
				// CASE 1: When the mouse is no longer hovering any piece
				RestoreLastHoveredPiece();
			}


		}
		else
		{
			m_tileHighlight.SetActive(false);

			// remove all hover effects
			// CASE 1: When the mouse is no longer hovering any piece (raycast misses)
			// RestoreLastHoveredPiece();

			// remove all hover effects from all pieces
			RestoreAllHoverEffects();
		}
	}

	// Add this method to your class:
	private void RestoreAllHoverEffects()
	{
		foreach (var kvp in m_originalMaterials)
		{
			GameObject piece = kvp.Key;
			if (piece != null)
			{
				Renderer renderer = piece.GetComponentInChildren<Renderer>();
				if (renderer != null)
				{
					renderer.material = kvp.Value;
				}
			}
		}
		m_lastPieceRenderer = null;
		m_lastHoveredPiece = null;
	}

	private void ExitState(GameObject movingPiece)
	{
		this.enabled = false;
		m_tileHighlight.SetActive(false);
		MoveSelector move = GetComponent<MoveSelector>();
		move.EnterState(movingPiece);
	}

	public void SelectPiece(GameObject currentSelectedPiece)
	{
		if (currentSelectedPiece == null || currentSelectedPiece == m_selectedPiece)
		{
			return;
		}
		// Deselect previous
		if (m_selectedPiece != null && m_originalMaterials.ContainsKey(m_selectedPiece))
		{
			Renderer selectedPieceRenderer = m_selectedPiece.GetComponentInChildren<Renderer>();
			if (selectedPieceRenderer != null && m_originalMaterial != null)
			{
				selectedPieceRenderer.material = m_originalMaterial;
			}
		}
		m_selectedPiece = currentSelectedPiece;
		Renderer renderer = currentSelectedPiece.GetComponentInChildren<Renderer>();
		if (renderer != null && m_selectedMaterial != null)
		{
			// Store original material if not already stored
			if (!m_originalMaterials.ContainsKey(m_selectedPiece))
				m_originalMaterials[m_selectedPiece] = renderer.material;
			renderer.material = m_selectedMaterial;
		}
	}

	public void EnterState()
	{
		enabled = true;
	}

	// Helper method to restore the last hovered piece's material
	private void RestoreLastHoveredPiece()
	{
		if (m_lastPieceRenderer != null && m_lastPieceRenderer.gameObject != m_selectedPiece)
		{
			GameObject lastPiece = m_lastPieceRenderer.gameObject;
			if (m_originalMaterials.ContainsKey(lastPiece))
			{
				m_lastPieceRenderer.material = m_originalMaterials[lastPiece];
			}
			m_lastPieceRenderer = null;
		}
	}
}

