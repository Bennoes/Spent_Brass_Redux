using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class MagazineBase
{
    public WeaponManager Weapon { get; set; }

    protected List<GameObject> AmmoList  { get; set; } = new List<GameObject>();

    protected Vector2[] AmmoPositions { get; set; }

    public GameObject MagazinePanel { get; set; }
    public GameObject AmmoIconHolder { get; set; }

    protected Vector2 worldBulletSize;
    protected Vector2 initialPosition;

    protected readonly float bulletSizeMod;

    protected Vector2 cachedAnchor = new(70, 400);


    public void SetUpMagazine()
    {
        ClearMagazine();

        PopulatePositionArray();

        CreateAmmoSprites();

        
    }

    protected abstract void PopulatePositionArray();

    protected void CreateAmmoSprites()
    {
        AmmoList.Clear();

        for (int i = 0; i < Weapon.State.currentAmmoCount; i++)
        {
            GameObject thisIcon = GameObject.Instantiate(AmmoIconHolder, MagazinePanel.transform);
            RectTransform holderRectTransform = thisIcon.GetComponent<RectTransform>();
            holderRectTransform.sizeDelta = worldBulletSize;

            thisIcon.transform.localPosition = AmmoPositions[i];

            Image iconImage = thisIcon.GetComponent<Image>();
            iconImage.sprite = Weapon.WeaponData.AmmoIcon;
            thisIcon.name = i.ToString();
            AmmoList.Add(thisIcon);
            thisIcon.transform.SetAsFirstSibling();

        }

    }

    protected void SetUpMagPanel()
    {
        if (MagazinePanel == null)
        {
            Debug.LogWarning("MagazinePanel is null");
            return;
        }

        RectTransform rectTransform = MagazinePanel.GetComponent<RectTransform>();

        ReSizeMagazine(rectTransform);

        //Debug.Log("rect anchor " + rectTransform.anchoredPosition);

        float xPos = 0;
        float yPos = rectTransform.rect.height/2;

        initialPosition = new Vector2(xPos, yPos);

        

    }


    public void ShootTopBullet()
    {
        GameObject shotBullet = AmmoList.FirstOrDefault();
        AmmoList.Remove(shotBullet);
        GameObject.Destroy(shotBullet);

        MoveBulletsToNextPosition();

    }

    protected abstract void MoveBulletsToNextPosition();
    

    public virtual void ReloadOneRound()
    {
        Debug.Log("reload one round");
    }


    protected void ClearMagazine()
    {
        Transform[] transfroms = MagazinePanel.GetComponentsInChildren<Transform>();

        for (int i = 1; i <= MagazinePanel.transform.childCount; i++)
        {
            GameObject.Destroy(transfroms[i].gameObject);
        }

    }

    protected virtual void ReSizeMagazine(RectTransform magRect)
    {
        //Debug.Log("running reposition method");
        float edgeBuffer = 10;

        float magazineWidth = worldBulletSize.x + (edgeBuffer * 2);
        float magazineHeight = ((worldBulletSize.y - worldBulletSize.x) * Weapon.WeaponData.maxAmmo) + worldBulletSize.y;

        Vector2 oldSize = magRect.sizeDelta;
        //Debug.Log("size delta " + oldSize);
        Vector2 newSize = new(magazineWidth, magazineHeight);

        // Maintain a fixed bottom position by adjusting the anchored position accordingly
        float heightDifference = newSize.y - oldSize.y;
        magRect.anchoredPosition += new Vector2(0, heightDifference / 2);

        // Apply the new size
        magRect.sizeDelta = newSize;

        // Cache the new position to avoid drift
        cachedAnchor = magRect.anchoredPosition;
    }


}
