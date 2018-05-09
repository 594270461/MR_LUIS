using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviours : MonoBehaviour {

    public static Behaviours instance;

    // the following variables are references to possile targets
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject cube;
    [HideInInspector] public GameObject gazedTarget;

    void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }

    /// <summary>
    /// Changes the color of the target GameObject by providing the name of the object and the name of the color
    /// </summary>
    public void ChangeTargetColor(string targetName, string colorName)
    {
        GameObject g = FindTarget(targetName);
        if (g != null)
        {
            Debug.Log("Changing color " + colorName + " to target: " + g.name);
            switch (colorName)
            {
                case "blue":
                case "azure":
                    g.GetComponent<Renderer>().material.color = Color.blue;
                    break;

                case "red":
                case "read":
                    g.GetComponent<Renderer>().material.color = Color.red;
                    break;

                case "yellow":
                    g.GetComponent<Renderer>().material.color = Color.yellow;
                    break;

                case "green":
                    g.GetComponent<Renderer>().material.color = Color.green;
                    break;

                case "white":
                    g.GetComponent<Renderer>().material.color = Color.white;
                    break;

                case "black":
                    g.GetComponent<Renderer>().material.color = Color.black;
                    break;

                case "purple":
                case "magenta":
                    g.GetComponent<Renderer>().material.color = Color.magenta;
                    break;

                case "gray":
                case "grey":
                    g.GetComponent<Renderer>().material.color = Color.gray;
                    break;

            }

        }
    }

    /// <summary>
    /// Reduces the size of the target GameObject by providing its name
    /// </summary>
    public void DownSizeTarget(string targetName)
    {
        GameObject g = FindTarget(targetName);
        g.transform.localScale -= new Vector3(0.5F, 0.5F, 0.5F);
    }

    /// <summary>
    /// Increases the size of the target GameObject by providing its name
    /// </summary>
    public void UpSizeTarget(string targetName)
    {
        GameObject g = FindTarget(targetName);
        g.transform.localScale += new Vector3(0.5F, 0.5F, 0.5F);
    }

    /// <summary>
    /// Determines which obejct reference is the target GameObject by providing its name
    /// </summary>
    private GameObject FindTarget(string name)
    {
        GameObject targetAsGO = null;
        switch (name)
        {
            case "sphere":
            case "ball":
                targetAsGO = sphere;
                break;

            case "cylinder":
            case "pipe":
            case "tube":
                targetAsGO = cylinder;
                break;

            case "cube":
            case "block":
                targetAsGO = cube;
                break;

            case "this": // as an example of target words that the user may use when looking at an object
            case "it":  // as this is the default, these are not actually needed in this example
            case "that":
            default: // if the target name is none of those above, check if the user is looking at something
                if (gazedTarget != null)
                {
                    targetAsGO = gazedTarget;
                }
                break;
        }

        return targetAsGO;
    }
}
