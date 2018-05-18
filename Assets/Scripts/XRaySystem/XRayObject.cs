using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayObject : MonoBehaviour {
    
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
    
    private const float alpha = 75;

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

            //Set renderlayers renderqueue to 2000 so that xray will have priority over it
            SetRenderQueue(renderLayer, 2000);
            
            //Make this layer ignored by xray
            ApplyShader(this.gameObject, xRayIgnoreShader);

            transparentLayer.transform.SetParent(this.transform);
            renderLayer.transform.SetParent(this.transform);
        }
        else
        {
            ApplyShader(this.gameObject, xRayRevealShader);
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
            Color c = m.material.color;
            c.a = alpha;
            m.material.SetColor("_MainColor", c);
        }
    }

    private void ApplyShader(GameObject g, Shader shader)
    {
        foreach (MeshRenderer m in g.GetComponentsInChildren<MeshRenderer>())
        {
            m.material.shader = shader;
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
}
