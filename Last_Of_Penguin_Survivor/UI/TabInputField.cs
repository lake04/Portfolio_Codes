using UnityEngine;
using TMPro;

public class TabInputField : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;
    public TMP_InputField nameInputField;

    private int inputSelected;

    void Start()
    {
        ipInputField.onSelect.AddListener((value) => IpSelected());
        portInputField.onSelect.AddListener((value) => PortSelected());
        nameInputField.onSelect.AddListener((value) => NameSelected());
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Tab))
        {
            inputSelected ++;
            if(inputSelected > 2 )
            {
                inputSelected = 0;
            }
            SelectInputField();
        }
    }

    private void SelectInputField()
    {
        switch(inputSelected)
        {
            case 0 :
                ipInputField.Select();
                break;
            case 1 :
                portInputField.Select();
                break;
            case 2 :
                nameInputField.Select();
                break;
           default:
                break;
        }
    }

    public void IpSelected() => inputSelected = 0;
    public void PortSelected() => inputSelected = 1;
    public void NameSelected() => inputSelected = 1;

}
