using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D _perlinNoiseTexture;
    [SerializeField] private int _maxHeight = 10;
    private MeshFilter _meshFilter;
    private int _TileSize = 1;
    private int _width = 256;
    private int _height = 256;
    private int _vertexCount;
    private int _tileCount;
    private Color[] _pixels;
    private Vector3[] _verticesPositions;
    private Vector2[] _texCoords;
    private int[] _triangles;
    public Gradient _gradient;
    private Color[] _vertexColors;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();


        _width = _perlinNoiseTexture.width;
        _height = _perlinNoiseTexture.height;
        _pixels = _perlinNoiseTexture.GetPixels();
        _vertexCount = _width * _height;
        _tileCount = (_width - 1) * (_height - 1);
        _verticesPositions = new Vector3[_width * _height];

        _triangles = new int[_tileCount * 2 * 3];
        computeVerticesPositions();
        computeTriangles();
        CalculateTextureCoords();
        CalculateMeshColor();

        Mesh mesh = new Mesh();
        _meshFilter.mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = _verticesPositions;
        mesh.triangles = _triangles;
        mesh.uv = _texCoords;
        mesh.colors = _vertexColors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

    }

    private void CalculateMeshColor()
    {
        _vertexColors = new Color[_verticesPositions.Length];
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _vertexColors[Index2DTo1D(i, j)] = _gradient.Evaluate(Index2DTo1D(i, j)/(float)_vertexCount);
            }
        }
    }

    private void CalculateTextureCoords()
    {
        _texCoords = new Vector2[_verticesPositions.Length];
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _texCoords[Index2DTo1D(i, j)] = PixelCoordsToLinear(new Vector2(i, j));
            }
        }
    }

        private void computeTriangles()
    {
        for (int i = 0; i < _width - 1; i++)
        {
            for (int j = 0; j < _height - 1; j++)
            {
                int tileIndex = TileIndexTo1D(i,j) * 6;
                _triangles[tileIndex] = Index2DTo1D(i, j);
                _triangles[tileIndex + 1] = Index2DTo1D(i, j+1);
                _triangles[tileIndex + 2] = Index2DTo1D(i+1, j);

                _triangles[tileIndex + 3] = Index2DTo1D(i+1, j);
                _triangles[tileIndex + 4] = Index2DTo1D(i, j+1);
                _triangles[tileIndex + 5] = Index2DTo1D(i + 1, j + 1);
            }
        }
    }

    private void computeVerticesPositions()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _verticesPositions[Index2DTo1D(i, j)] = Index2DToPosition(new Vector2(i, j));
            }
        }
    }

    private Vector2 PixelCoordsToLinear(Vector2 pixelCoords)
    {
        return new Vector2(pixelCoords.x / _width, pixelCoords.y / _height);
    }

    private int TileIndexTo1D(int i, int j)
    {
        return i + j * (_width - 1);
    }
    private int Index2DTo1D(int i, int j)
    {
        return i + j * _width;
    }

    private Vector3 Index2DToPosition(Vector2 index)
    {
        return new Vector3(index.x, _pixels[Index2DTo1D((int)index.x, (int)index.y)].grayscale * _maxHeight, index.y);
    }
}
