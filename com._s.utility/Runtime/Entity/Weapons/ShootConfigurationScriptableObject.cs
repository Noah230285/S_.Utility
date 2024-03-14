using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
public class ShootConfigurationScriptableObject : ScriptableObject{
    public LayerMask hitMask;
    //If the trigger must be released and held again before shooting
    public bool fullAuto = false;
    //Delay in seconds between shots
    public float fireRate = 0.15f;
    //Time to naturally return to where the player is aiming from recoil
    public float centreSpeed = 1f;
    //Time needed to be shooting before reaching maximum possible spread amount
    public float maxSpreadTime = 1f;
    //Which weapon spread system to use
    public BulletSpreadType spreadType = BulletSpreadType.Simple;
    [Header("Simple Spread")]
    public Vector3 spread = new Vector3 (0, 0, 0);
    [Header("Texture Based Spread")]
    [Range(0.001f, 5f)]
    public float spreadMultiplier = 0.1f;
    public Texture2D spreadTexture; 

    //Called when the weapon this config is attached to fires
    public Vector3 GetSpread(float shootTime = 0){
        Vector3 spread = Vector3.zero;

        if(spreadType == BulletSpreadType.Simple)
        {
            spread = Vector3.Lerp(
                Vector3.zero, new Vector3(
                Random.Range(-spread.x, spread.x),
                Random.Range(-spread.y, spread.y),
                Random.Range(-spread.z, spread.z)),
                Mathf.Clamp01(shootTime/maxSpreadTime));

            spread.Normalize();
        }
        else if(spreadType == BulletSpreadType.TextureBased)
        {
            spread = GetTextureDirection(shootTime);
            spread *= spreadMultiplier;
        }

        
        return spread;
    }

    //Called when using the texture based recoil system
    private Vector3 GetTextureDirection(float shootTime) {
        Vector2 halfSize = new Vector2(spreadTexture.width/2, spreadTexture.height/2);
        int halfSquareExtents = Mathf.CeilToInt(
            Mathf.Lerp(
                1,
                halfSize.x,
                Mathf.Clamp01(shootTime / maxSpreadTime)
            )
        );

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = spreadTexture.GetPixels(
            minX,
            minY,
            halfSquareExtents * 2,
            halfSquareExtents * 2
        );

        float[] colorsAsGray = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGrayVal = colorsAsGray.Sum();

        float gray = Random.Range(0, totalGrayVal);
        int i = 0;
        for(; i < sampleColors.Length; i++){
            gray -= colorsAsGray[i];
            if( gray <= 0) {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPos = new Vector2(x, y);
        Vector2 direction = (targetPos - halfSize) / halfSize.x;

        return direction;
    }
}
