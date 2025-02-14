using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public WeaponController weaponController;

    public GameObject equipedPanel;
    public GameObject holsteredPanel;

    public GameObject magazinePanel;
    public GameObject ammoIconHolder;

    private Image equippedImage;
    private Sprite ammoSprite;

    private GameObject[] ammoArray;
    // Start is called before the first frame update
    private Vector2[] ammoPositions; 
    
    void Start()
    {
        


 
    }

    // Update is called once per frame
    void Update()
    {
        if(equippedImage == null)
        {
            Sprite icon = weaponController.weaponInventory[0].WeaponData.weaponIcon;

            Image equippedImage = equipedPanel.GetComponent<Image>();

            equippedImage.sprite = icon;
        }
    }

    private void SetUpMagArray(float x, float y)
    {
        RectTransform rectTransform = magazinePanel.GetComponent<RectTransform>();
        Rect parentRect = rectTransform.rect;
      
        ammoPositions = new Vector2[weaponController.weaponInventory[0].WeaponData.maxAmmo];

        RectTransform ammorect = ammoIconHolder.GetComponent<RectTransform>();
        Vector2 scaledRectSize = new(ammorect.rect.width * x, ammorect.rect.height * y);

        
        float xOffset = scaledRectSize.x / 2;
        float yOffset = Mathf.Sqrt(Mathf.Pow(scaledRectSize.x, 2) - Mathf.Pow(scaledRectSize.x/2, 2)); //pythag

        //get position for first bullet
        float originAdjustX = parentRect.width / 2;
        float originAdjustY = parentRect.height;       
        Vector2 firstPos = new(parentRect.position.x + originAdjustX, parentRect.position.y + originAdjustY - ammorect.rect.width); //-rect width

        switch (weaponController.weaponInventory[0].WeaponData.weaponType)
        {
            case WeaponType.PISTOL:
                SetUpSingleStack(firstPos, scaledRectSize);

                break;
            case WeaponType.ASSUALT_RIFLE:

                SetUpStaggeredMag(firstPos,scaledRectSize,xOffset,yOffset);

                break;
            case WeaponType.SHOTGUN:
                break;
            case WeaponType.MACHINE_PISTOL:
                break;
            case WeaponType.LMG:
                break;
            case WeaponType.BATTLE_RIFLE:
                break;
            case WeaponType.DMR:
                break;
            default:
                break;
        }

    }

    private void SetUpStaggeredMag(Vector2 firstPos, Vector2 scaledRectSize, float xOffset, float yOffset)
    {
        for (int i = 0; i < ammoPositions.Length; i++)
        {

            if (i == 0)
            {
                ammoPositions[i] = firstPos;
                continue;
            }

            if (i == 1)
            {
                ammoPositions[i] = new(firstPos.x + (xOffset), firstPos.y - (yOffset * i));
                continue;
            }

            if (i % 2 == 0)  //even number
            {
                
                Vector2 previous = ammoPositions[i - 1];
                ammoPositions[i] = new(previous.x - ((MathF.Sqrt(3) / 2) * scaledRectSize.x), previous.y - scaledRectSize.x / 2);
            }
            else
            {
                Vector2 previous = ammoPositions[i - 1];
                ammoPositions[i] = new(previous.x + ((MathF.Sqrt(3) / 2) * scaledRectSize.x), previous.y - scaledRectSize.x / 2);
            }

        }
    }

    private void SetUpSingleStack(Vector2 firstPos, Vector2 scaledRectSize)
    {
        for (int i = 0; i < ammoPositions.Length; i++)
        {
            if(i == 0)
            {
                ammoPositions[i] = firstPos;
                continue;
            }

            float newYValue = firstPos.y - (scaledRectSize.x * i);

            ammoPositions[i] = new(firstPos.x, newYValue);

        }
    }


    private void OnEnable()
    {
        WeaponController.OnWeaponUpdate += UpdateWeaponUI;
        WeaponController.SetUpMag += SetUpMagazine;
        WeaponController.DepleteByOne += ShootOneRound;
        WeaponController.ReloadMag += ReloadMag;
        WeaponController.ReloadOneRound += ReloadOneRound;

        
    }
    private void OnDisable()
    {
        WeaponController.OnWeaponUpdate -= UpdateWeaponUI;
        WeaponController.SetUpMag -= SetUpMagazine;
        WeaponController.DepleteByOne -= ShootOneRound;
        WeaponController.ReloadMag -= ReloadMag;
        WeaponController.ReloadOneRound -= ReloadOneRound;
    }

    private void UpdateWeaponUI()
    {

        if (weaponController.weaponInventory[0] != null)
        {

            IconUpdate(0, equipedPanel);
        }

        if (weaponController.weaponInventory[1] != null)
        {
            IconUpdate(1, holsteredPanel);

        }
        else
        {
            Image iconImage = holsteredPanel.GetComponent<Image>();
            iconImage.sprite = null;
        }


    }

    private void IconUpdate(int invNumber, GameObject panel)
    {
        Sprite weaponIcon = weaponController.weaponInventory[invNumber].WeaponData.weaponIcon;
        Debug.Log("changing to " + weaponIcon.name);
        Image iconImage = panel.GetComponent<Image>();
        iconImage.sprite = weaponIcon;
    }

    private void ShootOneRound()
    {
        //update this to moving each bullet icon to the next position
        Debug.Log("Shoot one round");
        int maxChildren = magazinePanel.transform.childCount;
        GameObject child = magazinePanel.transform.GetChild(maxChildren - 1).gameObject;
        Destroy(child);

        switch (weaponController.weaponInventory[0].WeaponData.weaponType)
        {
            case WeaponType.PISTOL:
                Debug.Log("pistol");
                SingleStack(maxChildren);

                break;
            case WeaponType.ASSUALT_RIFLE:
                Debug.Log("assault Rifle");
                StaggeredBullets(maxChildren);

                break;
            case WeaponType.SHOTGUN:


                break;
            case WeaponType.MACHINE_PISTOL:
                break;
            case WeaponType.LMG:
                break;
            case WeaponType.BATTLE_RIFLE:
                break;
            case WeaponType.DMR:
                break;
            default:
                break;
        }

        
    }

    private void StaggeredBullets(int maxChildren)
    {
        for (int i = 0; i < maxChildren; i++)
        {
            GameObject sibling = magazinePanel.transform.GetChild(i).gameObject;
            RectTransform rectTransform = sibling.GetComponent<RectTransform>();

            if (i == maxChildren - 1) continue;

            if (i == maxChildren - 2)
            {
                Debug.Log("last pos");
                Debug.Log(rectTransform.localPosition);
                Debug.Log(ammoPositions[0]);
                LeanTween.moveLocal(sibling, ammoPositions[0], 0.1f);

            }
            else
            {
                int indexCorrected = maxChildren - i;
                LeanTween.moveLocalY(sibling, ammoPositions[indexCorrected - 1].y, 0.1f);

            }
        }
    }

    private void SingleStack(int maxChildren)
    {
        for (int i = 0; i < maxChildren; i++)
        {
            if (i == maxChildren - 1) continue;     //the highest value e.g top bullet thats being fired

            GameObject sibling = magazinePanel.transform.GetChild(i).gameObject;
            RectTransform rectTransform = sibling.GetComponent<RectTransform>();

            int indexCorrected = maxChildren - i;
            LeanTween.moveLocalY(sibling, ammoPositions[indexCorrected -2].y, 0.1f);

        }

    }

    private void PumpAction()
    {

    }



    private void SetUpMagazine()
    {
        Debug.Log("Set Up Mag");
        ClearMag();
        WeaponSO currentData = weaponController.weaponInventory[0].WeaponData;
        WeaponState currentState = weaponController.weaponInventory[0].State;

        

        //get size of sprite and ratio

        float spriteX = currentData.AmmoIcon.bounds.size.x * currentData.AmmoIcon.pixelsPerUnit;
        float spriteY = currentData.AmmoIcon.bounds.size.y * currentData.AmmoIcon.pixelsPerUnit;
        

        SetUpMagArray(spriteX, spriteY);

        Debug.Log("ammo sprite bounds " + spriteX + "," + spriteY);
               

        for (int i = 0; i < currentState.currentAmmoCount; i++)
        {

            GameObject iconHolder = Instantiate(ammoIconHolder, magazinePanel.transform);
            Image ammoImage = iconHolder.GetComponent<Image>();
            ammoImage.sprite = currentData.AmmoIcon;

            //change rect transform size to suit bullet
            RectTransform rectTransform = iconHolder.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x * spriteX, rectTransform.sizeDelta.y * spriteY);
            
            iconHolder.transform.localPosition = ammoPositions[i];
            iconHolder.transform.SetSiblingIndex(0);
        }

        

    }

    private void ClearMag()
    {
        Debug.Log("clear mag");
        for (int i = 0; i< magazinePanel.transform.childCount; i++)
        {
            Destroy(magazinePanel.transform.GetChild(i).gameObject);

        }
    }

    private void ReloadMag()
    {
        SetUpMagazine();

    }

    private void ReloadOneRound()
    {
        //make method of teh below
        WeaponSO currentData = weaponController.weaponInventory[0].WeaponData;
        Sprite ammoSprite = currentData.AmmoIcon;
        Vector2 spriteSize = ammoSprite.bounds.size;
        float pixelsPerUnit = ammoSprite.pixelsPerUnit;
        Vector2 spriteSizePixels = spriteSize * pixelsPerUnit;

        GameObject iconHolder = Instantiate(ammoIconHolder, magazinePanel.transform);
        Image ammoImage = iconHolder.GetComponent<Image>();
        ammoImage.sprite = currentData.AmmoIcon;
        RectTransform ammoHolderTransform = iconHolder.GetComponent<RectTransform>();
        //ammoHolderTransform.sizeDelta = spriteSizePixels;
    }


}



