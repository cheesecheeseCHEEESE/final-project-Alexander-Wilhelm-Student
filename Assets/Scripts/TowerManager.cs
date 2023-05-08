using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] GameObject[] towers;
    [SerializeField] Sprite[] towerImages;
    [SerializeField] float boxCastSize;
    [SerializeField] BoxCollider2D towerCheck;

    [SerializeField] AudioClip towerSelectSound;
    [SerializeField] AudioClip alreadySelectedSound;
    [SerializeField] AudioClip placeFailSound;
    [SerializeField] AudioClip insufficientCashSound;
    AudioPlayer audioPlayer;
    GameManager gm;
    UIScript uiscript;

    int towerIndex;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        TowerInfo tinfo = FindObjectOfType<TowerInfo>();
        if (tinfo != null)
        {
            towers = tinfo.towers.ToArray(); //surprise tool that will help us later
            towerImages = tinfo.towerImgs.ToArray();
        }

        uiscript = FindObjectOfType<UIScript>();
        for (int i = 0; i < towers.Length; i++)
        {
            uiscript.setupTowerButton(i, towers[i], towerImages[i]);
        }

        Destroy(tinfo); //don't need this no more!
    }



    public void PlaceTower()
    {
        int cashCost = towers[towerIndex].GetComponent<Health>().GetCashCost();
        if (cashCost <= gm.GetCash())
        {
            Vector3 pos = FindObjectOfType<UIScript>().GridButtonClick();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos); //get its position in the world
            worldPos = new Vector3(worldPos.x, worldPos.y, 0); //snap to zero 
            BoxCollider2D check = Instantiate(towerCheck, worldPos, Quaternion.identity);
            ContactFilter2D filter = new ContactFilter2D(); filter.SetLayerMask(LayerMask.GetMask("Tower"));
            //List<Collider2D> hits = new List<Collider2D>();
            if (check.OverlapCollider(filter, new List<Collider2D>()) == 0) //ensure there are no collisions
            {
                Instantiate(towers[towerIndex], worldPos, Quaternion.identity);
                gm.AddCash(-cashCost);
                uiscript.UpdateMoneyText();
            }
            else audioPlayer.PlayClip(placeFailSound, 1);
            Destroy(check.gameObject);
        }
        else audioPlayer.PlayClip(insufficientCashSound, 1);
    }

    public void setTowerIndex(int x)
    {
        if (x == towerIndex) audioPlayer.PlayClip(alreadySelectedSound, 1);
        else
        {
            int cashCost = towers[x].GetComponent<Health>().GetCashCost();
            if (cashCost <= gm.GetCash())
            {
                towerIndex = x;
                audioPlayer.PlayClip(towerSelectSound, 1);
            }
            else audioPlayer.PlayClip(insufficientCashSound, 1);
        }
    }
}
