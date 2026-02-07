using UnityEngine;
using TMPro;
using System.Collections;

public class TextShakeScript : MonoBehaviour
{
    [SerializeField] float intensity = 1f;
    private TextMeshProUGUI tmpText;
    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertixArr;
    private bool StopShaking = false;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void StartShake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeLettersRoutine());
    }

    IEnumerator ShakeLettersRoutine()
    {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;
        originalVertixArr = new Vector3[textInfo.meshInfo.Length][]; //store starting state

        for (int i = 0; i < textInfo.meshInfo.Length; i++) //clone all vertex data
        {
            originalVertixArr[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }

        while (!StopShaking)
        {
            tmpText.ForceMeshUpdate();

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue; //skip unrevealed text

                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;

                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity, 0f);

                //apply to the 4 corners
                vertices[vertIndex] += randomOffset;
                vertices[vertIndex + 1] += randomOffset;
                vertices[vertIndex + 2] += randomOffset;
                vertices[vertIndex + 3] += randomOffset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }

        //restore original state
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = originalVertixArr[i];
            tmpText.UpdateGeometry(meshInfo.mesh, i);
        }

        tmpText.ForceMeshUpdate();
        StopShaking = false;


    }

    public void StopShake()
    {
        StopShaking = true;
    }
}
