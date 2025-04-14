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
    //new stuff for strategy pattern
    public Dictionary<MagazineType, MagazineBase> magazineDictionary;


    public WeaponController weaponController;

    public GameObject equipedPanel;
    public GameObject holsteredPanel;

    public GameObject magazinePanel;
    public GameObject ammoIconHolder;

    private Image equippedImage;
    

    private List<GameObject> ammoList;
    // Start is called before the first frame update
    private Vector2[] ammoPositions;

    private void Awake()
    {
        SingleStackMag singleStackMag = new SingleStackMag();
        TubeMag tubeMag = new TubeMag();
        DoubleStackMag doubleStackMag = new DoubleStackMag();


        magazineDictionary = new Dictionary<MagazineType, MagazineBase>
        {
            {MagazineType.SINGLE_STACK, singleStackMag },
            {MagazineType.DOUBLE_STACK, doubleStackMag },
            {MagazineType.TUBE, tubeMag }
        };
    }

    void Start()
    {
        
        //set up dictionary of ammo concrete classes

        
    }

    private void SetUpMagazine()
    {
        WeaponManager weaponManager = weaponController.weaponInventory[0];


        MagazineBase thisMag = magazineDictionary[weaponManager.WeaponData.magazineType];

        //set properties in magazine class
        thisMag.Weapon = weaponManager;
        //check for null. if null assign a value. if not leave as it is
        thisMag.MagazinePanel = thisMag.MagazinePanel != null ? thisMag.MagazinePanel : magazinePanel;
        thisMag.AmmoIconHolder = thisMag.AmmoIconHolder != null ? thisMag.AmmoIconHolder : ammoIconHolder;

        thisMag.SetUpMagazine();

    }


    private void SwapMagazine()
    {       
        SetUpMagazine();
    }

    

    private void ShootBullet()
    {
        //try caching equipped data - will use it quite a bit. Maybr cache thisMag too
        //destroy panel at top of mag
        WeaponSO equippedData = weaponController.weaponInventory[0].WeaponData;
        MagazineBase thisMag = magazineDictionary[equippedData.magazineType];
        thisMag.ShootTopBullet();

        //move each bullet up to next position

        //thisMag.MoveBulletsToNextPosition(ammoList, ammoPositions, equippedData);

    }

    private void ReloadOneRound()
    {
        WeaponManager manager = weaponController.weaponInventory[0];
        WeaponSO equippedData = manager.WeaponData;
        MagazineBase thisMag = magazineDictionary[equippedData.magazineType];
        //Debug.Log("reloading shotty");
        thisMag.ReloadOneRound();
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

    private void InitialSetUp()
    {
        //Debug.Log("method called from set up event in weapon Controller");

        SetUpMagazine();
    }


    private void OnEnable()
    {
       WeaponController.OnWeaponUpdate += SwapMagazine;
       // WeaponController.SetUpMag += SetUpMagazine;
        WeaponController.DepleteByOne += ShootBullet;
       WeaponController.ReloadMag += SwapMagazine;
       WeaponController.ReloadOneRound += ReloadOneRound;

        WeaponController.OnInitialSetUp += InitialSetUp;


    }
    private void OnDisable()
    {
       // WeaponController.OnWeaponUpdate -= UpdateWeaponUI;
       // WeaponController.SetUpMag -= SetUpMagazine;
        WeaponController.DepleteByOne -= ShootBullet;
        WeaponController.ReloadMag -= SwapMagazine;
        WeaponController.ReloadOneRound -= ReloadOneRound;
    }



}



