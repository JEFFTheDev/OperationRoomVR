using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class XRayObject : MonoBehaviour {

    private enum WorkflowMode
    {
        Specular,
        Metallic,
        Dielectric
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }

    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }

    //Shader names
    private const string xRay = "Custom/Stencil/XRay";
    private const string xRayIgnore = "Custom/Stencil/XRayIgnore";
    private const string xRayReveal = "Custom/Stencil/XRayContents";
    private const string xRayIgnoreRenderer = "Custom/Stencil/XRayIgnoreRenderer";
    

    //Shaders
    private Shader xRayShader;
    private Shader xRayIgnoreShader;
    private Shader xRayRevealShader;
    private Shader xRayIgnoreRendShader;
    
    private const float alpha = .4f;

    public bool isTransparent;
    
	// Use this for initialization
	void Start () {
        FindShaders();

        if (isTransparent)
        {
            //Create new layer and make it transparent
            GameObject transparentLayer = CreateObjectShaderLayer(this.gameObject, xRayRevealShader);
            transparentLayer.transform.position = transform.position;
            transparentLayer.transform.rotation = transform.rotation;
            transparentLayer.name = "TransparentLayer";
            MakeTransparent(transparentLayer);

            //Create new layer to render this one
            GameObject renderLayer = CreateObjectShaderLayer(this.gameObject, xRayIgnoreRendShader);
            renderLayer.name = "RenderLayer";
            
            Vector3 pos = renderLayer.transform.position;

            //Make it slightly bigger so its layered on top of this object
            //renderLayer.transform.localScale *= 1.05f;
            renderLayer.transform.position = pos;

            renderLayer.transform.rotation = transform.rotation;
            
            //Make this layer ignored by xray
            ApplyShader(this.gameObject, xRayIgnoreShader);

            //Set render queue of this object above renderlayer or it won't render
            //SetRenderQueue(this.gameObject, 2001);

            transparentLayer.transform.SetParent(this.transform);
            renderLayer.transform.SetParent(this.transform);

            //Set renderlayers renderqueue to 2001 so that xray will have priority over it
            SetRenderQueue(renderLayer, 1999);
            //SetRenderQueue(transparentLayer, 2000);
            //SetRenderQueue(this.gameObject, 1999);

            //UpdateMaterials(renderLayer);
            UpdateMaterials(transparentLayer);
            UpdateMaterials(this.gameObject);
        }
        else
        {
            ApplyShader(this.gameObject, xRayRevealShader);
            //SetRenderQueue(this.gameObject, 2000);
            UpdateMaterials(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private GameObject CreateObjectShaderLayer(GameObject g, Shader shader)
    {
        GameObject layer = GameObject.Instantiate(g);

        Destroy(layer.GetComponent<XRayObject>());

        foreach(MonoBehaviour m in layer.GetComponentsInChildren<MonoBehaviour>())
        {
            //TODO Remove all monobehaviours that aren't meshfilters or meshrenderers
        }

        ApplyShader(layer, shader);

        return layer;
    }

    private void MakeTransparent(GameObject g)
    {
        foreach(MeshRenderer m in g.GetComponentsInChildren<MeshRenderer>())
        {
            m.material.SetFloat("_Mode", 3);
            Color c = m.material.color;
            c.a = alpha;
            m.material.color = c;
        }
    }

    private void ApplyShader(GameObject g, Shader shader)
    {
        foreach (MeshRenderer m in g.GetComponentsInChildren<MeshRenderer>())
        {
            Material copy = new Material(shader);
            copy.CopyPropertiesFromMaterial(m.material);
            //copy.shader = shader;
            m.material = copy;
        }
    }
    
    private void SetRenderQueue(GameObject g, int queue)
    {
        foreach(MeshRenderer m in g.GetComponentsInChildren<MeshRenderer>())
        {
            m.material.renderQueue = queue;
        }
    }

    private void FindShaders()
    {
        xRayShader = Shader.Find(xRay);
        xRayIgnoreShader = Shader.Find(xRayIgnore);
        xRayRevealShader = Shader.Find(xRayReveal);
        xRayIgnoreRendShader = Shader.Find(xRayIgnoreRenderer);
    }

    private void UpdateMaterials(GameObject g)
    {
        foreach (MeshRenderer m in g.GetComponentsInChildren<MeshRenderer>())
        {
            MaterialChanged(m.material, WorkflowMode.Metallic);
        }
    }

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

    static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
    {
        int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
        if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
            return SmoothnessMapChannel.AlbedoAlpha;
        else
            return SmoothnessMapChannel.SpecularMetallicAlpha;
    }

    static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
    {
        // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
        // (MaterialProperty value might come from renderer material property block)
        SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
        if (workflowMode == WorkflowMode.Specular)
            Debug.Log("Continue");//SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
        else if (workflowMode == WorkflowMode.Metallic)
            SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
        SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
        SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

        // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
        // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
        // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
        MaterialEditor.FixupEmissiveFlag(material);
        bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
        SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

        if (material.HasProperty("_SmoothnessTextureChannel"))
        {
            SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
        }
    }

    static void MaterialChanged(Material material, WorkflowMode workflowMode)
    {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

        SetMaterialKeywords(material, workflowMode);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}
