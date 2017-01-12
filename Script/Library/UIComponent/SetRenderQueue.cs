#region Class Information
/***************************************************************
 * File			：	SetRenderQueue.cs
 * Description	：	设置物体的渲染队列，可以设置在NGUI前显示，NGUI从3000开始
 * Author		：	fhl
 * Date			：	20140623
 * Version		：	V 0.1.0	
 **************************************************************/
#endregion

using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    /// <summary>
    /// 渲染队列
    /// </summary>
    public int RenderQueue;

    public int order = 0;

    public bool autoChangeSize = false;

    //public string sortingLayerName = "Layer1";

    private UIPanel uiPanel;
    private UIWidget uiWidget;
    private Renderer mRenderer;

    private const int parentSearchDepth = 30;

    private float particleSize = 1;

    private float realSize = 0;
    private bool alreadyChange = false;

    public float panelQueue;
    public float widgetQueue;

    void Awake()
    {
        mRenderer = this.GetComponent<Renderer>();
        ChangeParticleSize();
    }

    void Start()
    {
        if(this.order != 0)
        {
            FindPanelAndWidget();
        }
    }

    public void SetParticleSize(float size)
    {
        this.particleSize = size;
        GetComponent<ParticleSystem>().startSize *= this.particleSize;
        realSize = GetComponent<ParticleSystem>().startSize;
    }

    public void RefreshParticleSize()
    {
        ChangeParticleSize();
        if (realSize > 0)
        {
            GetComponent<ParticleSystem>().startSize = realSize;
        }
    }

    void ChangeParticleSize()
    {
        if (alreadyChange) return;
        alreadyChange = true;

        if (!autoChangeSize) return;
        if(this.GetComponent<ParticleSystem>() == null) return;

        GetComponent<ParticleSystem>().startSize *= WindowUtility.GetAutoAdpateSize();

        realSize = GetComponent<ParticleSystem>().startSize;
    }

    void OnDestroy()
    {
        if(this.order != 0)
        {
            if (this.uiPanel != null)
            {
                this.uiPanel.removeUISort(this);
            }
        }
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        GetComponent<Renderer>().sharedMaterial.renderQueue = RenderQueue;
    }

    [ContextMenu("RefreshRenderQueue")]
    public void RefreshRenderQueue()
    {
        if(this.order != 0)
        {
            FindPanelAndWidget();
            UpdateRenderQueue(); 
        }
    }

    void FindPanelAndWidget()
    {
        this.uiPanel = FindParentPanel();
        if (this.uiPanel != null)
        {
            this.uiWidget = FindParentWidget(this.uiPanel);
            this.uiPanel.addUISort(this);
        }
    }


    public void UpdateRenderQueue()
    {
        if (this.uiWidget != null && uiWidget.drawCall != null)
        {
            this.mRenderer.material.renderQueue = this.uiWidget.drawCall.renderQueue + order;
        }
        else if(this.uiPanel != null)
        {
            this.mRenderer.material.renderQueue = this.uiPanel.finalRenderQueue + order; 
        }
    }


    UIWidget FindParentWidget(UIPanel uiPanel)
    {
        int depth = parentSearchDepth;
        int index = 0;
        Transform widgetTrans = null;
        UIWidget ret = null;

        widgetTrans = transform;

        while (ret == null && widgetTrans != null && widgetTrans.gameObject != uiPanel.gameObject)
        {
            if (index > depth)
                break;

            ret = GetUIWidget(widgetTrans);
            index++;

            widgetTrans = widgetTrans.parent;
        }

        return ret;
    }

    UIWidget GetUIWidget(Transform trans)
    {
        UIWidget ret = null;
        ret = trans.GetComponent<UIWidget>();

        UITexture texture = ret as UITexture;
        UILabel label = ret as UILabel;
        UISprite sprite = ret as UISprite;
        if (ret != null)
        {
            if (texture == null && label == null && sprite == null)
            {
                //Debug.LogWarning("特效的挂载点UIWidget无效，请确认该UIWidget是否能正常显示, 特效:" + gameObject.name + " UIWidget:" + ret.name);
                return null;
            }
        }
        return ret;
    }

    UIPanel FindParentPanel()
    {
        int depth = parentSearchDepth;
        int index = 0;
        Transform widgetTrans = null;
        UIPanel ret = null;

        widgetTrans = transform.parent;
        while (ret == null && widgetTrans != null)
        {
            if (index > depth)
                break;

            ret = widgetTrans.GetComponent<UIPanel>();
            index++;

            widgetTrans = widgetTrans.parent;
        }
        return ret;
    }
}
