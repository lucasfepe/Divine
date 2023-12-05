using Mono.CSharp.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        List<GlowShaderController> icons = new List<GlowShaderController>();
        GameObject newIcon = Instantiate(iconTemplate, transform);
        StatusInfo statusInfo = newIcon.GetComponent<StatusInfo>();
        statusInfo.SetStatusType(status.statusType);
        newIcon.SetActive(true);
        GameObject statusIcon = newIcon.GetComponentInChildren<GlowShaderController>().gameObject;
        GlowShaderController rockShaderController = statusIcon.GetComponentInChildren<GlowShaderController>();
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
            case EffectTypeEnum.NoBlackHole:
                material = Resources.Load<Material>("NoBlackHoleStatusMaterial");
                //create copy
                materialCopy = new Material(material);
                image.material = materialCopy;
                rockShaderController.SetMaterial(materialCopy);
                break;
            case EffectTypeEnum.Dim:
                material = Resources.Load<Material>("DimStatusMaterial");
                //create copy
                materialCopy = new Material(material);
                image.material = materialCopy;
                rockShaderController.SetMaterial(materialCopy);
                break;
            case EffectTypeEnum.Gemini:
                material = Resources.Load<Material>("GeminiStatusMaterial");
                //create copy
                materialCopy = new Material(material);
                image.material = materialCopy;
                rockShaderController.SetMaterial(materialCopy);
                break;
            case EffectTypeEnum.Untargetable:
                material = Resources.Load<Material>("UntargetableStatusMaterial");
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
            
            icons.Add(statusIcon.GetComponent<GlowShaderController>());
        }
        icons.ForEach(x => x.GlowFastAndBright());

        
    }
    public void RemoveStatusIcon(EffectTypeEnum effectType) {


        //the first one is the template
        Destroy(transform.GetComponentsInChildren<StatusInfo>().ToList().Where(x => x.GetStatusType() == effectType).First().gameObject);
    }

}
