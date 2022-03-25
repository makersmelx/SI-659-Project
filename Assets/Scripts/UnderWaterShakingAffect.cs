using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterShakingAffect : MonoBehaviour
{

    public Material underwater_shaking_mat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        underwater_shaking_mat.SetFloat("_Color",gameObject.transform.rotation.x);
        Graphics.Blit(source, destination, underwater_shaking_mat);
    }
}
