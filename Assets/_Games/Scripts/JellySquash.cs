using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class JellySquash : MonoBehaviour
{
    private Mesh originalMesh;
    private Mesh deformedMesh;
    private Vector3[] baseVertices;
    private Vector3[] currentVertices;
    private Vector3[] vertexVelocities;

    private Vector3 previousPosition;

    [Header("Jelly Settings")]
    public float inertiaStrength = 0.5f;   // Cường độ quán tính
    public float elasticity = 5f;          // Độ đàn hồi
    public float damping = 0.85f;          // Tỉ lệ giảm chuyển động

    [Header("Deformation Limits")]
    public float maxDeformation = 0.3f;    // Độ biến dạng tối đa (đơn vị khoảng cách)

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        deformedMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = deformedMesh;

        baseVertices = originalMesh.vertices;
        currentVertices = new Vector3[baseVertices.Length];
        vertexVelocities = new Vector3[baseVertices.Length];

        for (int i = 0; i < baseVertices.Length; i++)
            currentVertices[i] = baseVertices[i];

        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 worldDelta = transform.position - previousPosition;
        Vector3 localDelta = transform.InverseTransformDirection(worldDelta);

        // Tìm min/max để chuẩn hóa theo trục X, Y, Z
        float minY = float.MaxValue, maxY = float.MinValue;
        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (Vector3 v in baseVertices)
        {
            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }

        float height = maxY - minY;
        float width = maxX - minX;
        float depth = maxZ - minZ;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 original = baseVertices[i];

            float heightFactor = (original.y - minY) / height;
            float sideFactorX = (original.x - minX) / width;
            float sideFactorZ = (original.z - minZ) / depth;

            // Offset chính theo hướng chuyển động và độ cao
            Vector3 inertiaOffset = -localDelta * inertiaStrength * heightFactor;

            // Bẻ đầu đỉnh theo trục X (trái/phải) và Z (trước/sau)
            float horizontalBend = -localDelta.x * (0.5f - sideFactorX) * heightFactor;
            float depthBend = -localDelta.z * (0.5f - sideFactorZ) * heightFactor;

            inertiaOffset.y += horizontalBend;
            inertiaOffset.y += depthBend;

            // Tính offset dự đoán
            Vector3 predictedOffset = currentVertices[i] + vertexVelocities[i] * Time.deltaTime - original;

            // Giới hạn độ biến dạng
            if (predictedOffset.magnitude > maxDeformation)
            {
                predictedOffset = predictedOffset.normalized * maxDeformation;
                currentVertices[i] = original + predictedOffset;
                vertexVelocities[i] = Vector3.zero;
            }
            else
            {
                // Áp dụng vật lý
                vertexVelocities[i] += (inertiaOffset - (currentVertices[i] - original)) * elasticity * Time.deltaTime;
                vertexVelocities[i] *= damping;
                currentVertices[i] += vertexVelocities[i] * Time.deltaTime;
            }
        }

        deformedMesh.vertices = currentVertices;
        deformedMesh.RecalculateNormals();

        previousPosition = transform.position;
    }
}
