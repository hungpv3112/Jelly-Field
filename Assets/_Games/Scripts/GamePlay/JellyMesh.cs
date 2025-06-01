using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    [SerializeField] private float _intensity = 1.0f;
    [SerializeField] private float _mass = 1f;
    [SerializeField] private float _damping = 0.75f;
    [SerializeField] private float _stiffness = 1f;

    private Mesh _originalMesh;
    private Mesh _meshClone;
    private MeshRenderer _meshRenderer;
    private JellyVertex[] _jv;
    private Vector3[] _vertexArray;

    void Awake()
    {
        
    }

    void Start()
    {
        _originalMesh = GetComponent<MeshFilter>().sharedMesh;
        _meshClone = Instantiate(_originalMesh);
        GetComponent<MeshFilter>().mesh = _meshClone;
        _meshRenderer = GetComponent<MeshRenderer>();

        _jv = new JellyVertex[_meshClone.vertices.Length];
        for (int i = 0; i < _meshClone.vertices.Length; i++)
        {
            _jv[i] = new JellyVertex(i, transform.TransformPoint(_meshClone.vertices[i]));
        }
    }

    [Button]
    public void LogMeshId()
    {
        Debug.Log($"Mesh ID: {_meshClone.GetInstanceID()}");
        Debug.Log($"Original Mesh ID: {_originalMesh.GetInstanceID()}");

        var meshFilter = GetComponent<MeshFilter>();
        Debug.LogWarning($"MeshFilter ID: {meshFilter.sharedMesh.GetInstanceID()}");
        Debug.LogWarning($"MeshFilter instance ID: {meshFilter.mesh.GetInstanceID()}");

        var meshSlice = GetComponent<MeshSlice>();
        Debug.LogError($"MeshSlice ID: {meshSlice.originalMesh.GetInstanceID()}");
        Debug.LogError($"MeshSlice instance ID: {meshSlice.instance.GetInstanceID()}");

    }

    void LateUpdate()
    {
        _vertexArray = _originalMesh.vertices;
        for (int i = 0; i < _jv.Length; i++)
        {
            Vector3 targetPosition = transform.TransformPoint(_vertexArray[_jv[i].id]);
            float intensity = (1 - (_meshRenderer.bounds.max.y - targetPosition.y) / _meshRenderer.bounds.size.y) * _intensity;
            _jv[i].Shake(targetPosition, _mass, _stiffness, _damping);
            targetPosition = transform.InverseTransformPoint(_jv[i].position);
            _vertexArray[_jv[i].id] = Vector3.Lerp(_vertexArray[_jv[i].id], targetPosition, intensity);
        }

        _meshClone.vertices = _vertexArray;
    }

    public class JellyVertex
    {
        public int id;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 force;

        public JellyVertex(int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
            this.velocity = Vector3.zero;
            this.force = Vector3.zero;
        }

        public void Shake(Vector3 target, float m, float s, float d)
        {
            force = (target - position) * s;
            velocity = (velocity + force / m) * d;
            position += velocity;
            if ((velocity + force + force / m).magnitude < 0.001f)
            {
                position = target;
            }

        }
    }
}
