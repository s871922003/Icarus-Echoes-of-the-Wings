using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    public Color ActiveColor;
    public Color DeactiveColor;
    public SpriteRenderer Renderer;

    private void Start()
    {
        Renderer.color = DeactiveColor;
    }

    public void SwitchColor()
    {
        if(Renderer.color == ActiveColor)
        {
            Renderer.color = DeactiveColor;
        }
        else
        {
            Renderer.color = ActiveColor;
        }
    }

}
