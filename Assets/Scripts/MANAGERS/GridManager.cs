using UnityEngine;

namespace YourGameNamespace
{
    public class GridManager
    {
        private DungeonSettings dungeonSettings;

        public GridManager(DungeonSettings settings)
        {
            dungeonSettings = settings;
        }

        /// <summary>
        /// Adjusts the grid's position to ensure proper alignment with the world.
        /// </summary>
        /// <param name="gridParent">The parent GameObject of the grid (e.g., dungeonParent).</param>
        public void AdjustGridPosition(GameObject gridParent)
        {
            if (gridParent != null)
            {
                gridParent.transform.position += new Vector3(-0.5f, -0.5f, 0);
                Debug.Log("Grid position adjusted by (-0.5, -0.5).");
            }
            else
            {
                Debug.LogError("Grid parent is not assigned. Cannot adjust grid position.");
            }
        }

        /// <summary>
        /// Aligns a position to the nearest grid point.
        /// </summary>
        /// <param name="position">The world position to align.</param>
        /// <param name="cellSize">The size of a single grid cell.</param>
        /// <returns>The aligned position.</returns>
        public Vector3 AlignToGrid(Vector3 position, float cellSize = 1.0f)
        {
            float x = Mathf.Round(position.x / cellSize) * cellSize;
            float y = Mathf.Round(position.y / cellSize) * cellSize;
            return new Vector3(x, y, position.z);
        }

        /// <summary>
        /// Ensures a GameObject is snapped to the nearest grid cell.
        /// </summary>
        /// <param name="obj">The GameObject to snap.</param>
        public void SnapToGrid(GameObject obj)
        {
            Vector3 cellSize = GetCellSize(obj);
            obj.transform.position = AlignToGrid(obj.transform.position, cellSize.x);
        }

        /// <summary>
        /// Retrieves the cell size of the grid.
        /// </summary>
        /// <param name="gridObject">The GameObject containing the grid component.</param>
        /// <returns>The size of a single grid cell.</returns>
        public Vector3 GetCellSize(GameObject gridObject)
        {
            Grid grid = gridObject.GetComponent<Grid>();
            if (grid != null)
            {
                return grid.cellSize;
            }

            Debug.LogWarning("Grid component not found. Returning default cell size (1, 1, 1).");
            return Vector3.one;
        }

        /// <summary>
        /// Converts a world position to a grid position.
        /// </summary>
        /// <param name="worldPosition">The position in world coordinates.</param>
        /// <param name="cellSize">The size of a single grid cell.</param>
        /// <returns>The corresponding grid position.</returns>
        public Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize = 1.0f)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x / cellSize),
                Mathf.FloorToInt(worldPosition.y / cellSize),
                Mathf.FloorToInt(worldPosition.z / cellSize)
            );
        }

        /// <summary>
        /// Converts a grid position to a world position.
        /// </summary>
        /// <param name="gridPosition">The position in grid coordinates.</param>
        /// <param name="cellSize">The size of a single grid cell.</param>
        /// <returns>The corresponding world position.</returns>
        public Vector3 GridToWorldPosition(Vector3Int gridPosition, float cellSize = 1.0f)
        {
            return new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                gridPosition.z * cellSize
            );
        }

        /// <summary>
        /// Validates whether a given position is within grid bounds.
        /// </summary>
        /// <param name="position">The position to validate.</param>
        /// <param name="gridBounds">The bounds of the grid area.</param>
        /// <returns>True if the position is valid, false otherwise.</returns>
        public bool IsPositionWithinGridBounds(Vector3 position, Bounds gridBounds)
        {
            return gridBounds.Contains(position);
        }

        /// <summary>
        /// Calculates the world bounds of the grid based on its size and position.
        /// </summary>
        /// <param name="gridObject">The grid GameObject.</param>
        /// <param name="gridSize">The size of the grid in cells.</param>
        /// <returns>The bounds of the grid in world coordinates.</returns>
        public Bounds CalculateGridBounds(GameObject gridObject, Vector2Int gridSize)
        {
            Vector3 cellSize = GetCellSize(gridObject);
            Vector3 worldCenter = gridObject.transform.position;
            Vector3 worldSize = new Vector3(gridSize.x * cellSize.x, gridSize.y * cellSize.y, cellSize.z);

            return new Bounds(worldCenter, worldSize);
        }
    }
}
