using Mono.CSharp.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconsUI : MonoBehaviour
{
    [SerializeField] private GameObject iconTemplate;

    private void Awake()
    {
        iconTemplate.SetActive(false);
    }

    public void AddStatusIcon(Status status)
    {
        List<RockShaderController> icons = new List<RockShaderController>();
        GameObject newIcon = Instantiate(iconTemplate, transform);
        newIcon.SetActive(true);
        GameObject statusIcon = newIcon.GetComponentInChildren<RockShaderController>().gameObject;
        RockShaderController rockShaderController = statusIcon.GetComponentInChildren<RockShaderController>();
        icons.Add(rockShaderController);
        Image image = statusIcon.GetComponentInChildren<Image>();
        image.sprite = Image2SpriteUtility.Instance.GetSkillEffectIconSprite(status.statusType);
        Material material = null;
        Material materialCopy = null;
        //need to assign copy material or will change every object that uses this material instead of jut this
        switch (status.statusType)
        {
            case EffectTypeEnum.Strange:
                material = Resources.Load<Material>("StrangeStatusMaterial");
                //create copy
                materialCopy = new Material(material);
                image.material = materialCopy;
                rockShaderController.SetMaterial(materialCopy);
                break;
                //case EffectTypeEnum.BonusEnergy:
                //     material = Resources.Load<Material>("InspiredStatusMaterial");
                //    //create copy
                //    materialCopy = new Material(material);
                //    image.material = materialCopy;
                //    rockShaderController.SetMaterial(materialCopy);
                //    break;
                //case EffectTypeEnum.GainExperience:
                //     material = Resources.Load<Material>("WisdomStatusMaterial");
                //    materialCopy = new Material(material);
                //    image.material = materialCopy;
                //    rockShaderController.SetMaterial(materialCopy);
                //    break;
        }



        for (int i = 1; i < status.statusPower; i++)
        {
            GameObject newIconLevel = Instantiate(statusIcon, newIcon.transform);
            newIconLevel.GetComponent<Image>().material = materialCopy;
            RectTransform rt = newIconLevel.GetComponent<RectTransform>();
            float offset = -5f;
            rt.offsetMin = new Vector2(i * offset, i * offset);
            rt.offsetMax = new Vector2(-i * offset, -i * offset);
            newIconLevel.transform.SetSiblingIndex(newIconLevel.transform.GetSiblingIndex() - i);
            
            icons.Add(statusIcon.GetComponent<RockShaderController>());
        }
        icons.ForEach(x => x.GlowFastAndBright());

        
    }
    public void RemoveStatusIcon(int index) {
        //the first one is the template
        Destroy(transform.GetChild(index + 1).gameObject);
    }

}
