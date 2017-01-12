// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LayerUtility.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class LayerUtility
{

    public static void SetLayer(GameObject gameobject, int layer)
    {
        gameobject.layer = layer;

        Transform trans = gameobject.transform;

        for (int i = 0, imax = trans.childCount; i < imax; ++i)
        {
            Transform child = trans.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }


    public static void ResetShader(GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
        Renderer renderer;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderer = renderers[i];
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.shader = Shader.Find(renderer.sharedMaterial.shader.name);
            }
        }

        ParticleSystemRenderer[] particleRenderers = gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
        ParticleSystemRenderer particleRenderer;
        for (int i = 0; i < particleRenderers.Length; i++)
        {
            particleRenderer = particleRenderers[i];
            if (particleRenderer.sharedMaterial != null)
            {
                particleRenderer.sharedMaterial.shader = Shader.Find(particleRenderer.sharedMaterial.shader.name);
            }
        }
    }
}

