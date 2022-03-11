using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class HaalrenScript : BaseCharacterClass
{

    public GameObject[] testObjs;
    public struct MoveSetStruct
    {
        public MoveSetStruct(Action _moveAction, float _timeTilNextMove)
        {
            this.moveAction = _moveAction;
            this.timeTilNextMove = _timeTilNextMove;
        }
        public Action moveAction;
        public float timeTilNextMove;
    }
    //Declreations
    [Header("General Declreations")]
    public int phases = 3;
    private GameObject player;
    private Animator anim;

    //Lazer Attacks
    [Header("LazerAttack")]
    public GameObject lazerObjectPrefab; //Prefab of the lazer object
    private GameObject lazerGameObject;
    public float maxLazerTime;
    [Tooltip("Time to do one full rotation, set as a factor of Max Lazer Time for best results")]
    public float lazerRotSpeed;
    
    //Bullet Hell attack varibles
    [Header("Bullet Hell Attacks")]
    public GameObject bullet; //Used to instantiate Bullets
    public float amountOfBullets;
    public float bulletCircleRadius;
    public float bulletSpeed;
    private enum BulletStyles
    {
        Circular,
        Spiral,
        Pentagram
    }

    [Header("Line Bullet Attack")] 
    public int amountOfLineBullets;
    public int amountOfLines;
    public float timeBetweenLines;
    public GameObject leftSpawnObject, rightSpawnObject;

    [Header("Orb Towards Player Attack")] 
    public float orbSpeedMult;
    public GameObject orbSpawnLocation;
    public int amountOfOrbShots;
    public float timeBetweenOrbs;
    
    //Bomb Attack
    [Header("Bomb Attack")] 
    public GameObject bombPrefab;
    public float bombSpawnTimer;
    public float bombBlowupTimer;
    public float bombRadius;
    public int bombDamage;
    
    //Wall Attack
    [Header("Wall Attack")] 
    public List<GameObject> horizontalWallList;
    public List<GameObject> verticalWallList;
    public float wallTimer;

    //Ad Phases
    [Header("Healer Settings")] 
    public GameObject[] AdObjectsFirst, AdObjectsSecond;
    public GameObject First_Ads, Second_Ads;
    public bool FirstAdBool, SecondAdBool;

    
    //Next move sets
    private Queue<MoveSetStruct> nextMoveQueue = new Queue<MoveSetStruct>();

    [Header("Sounds")] 
    public AudioSource spawnBulletPentagramSFX;

    public AudioSource fireBulletSFX;
    public AudioSource lazerAttackSFX;
    private bool hasBeenActivated = false;


    [Header("Damage Mats")] public Material normalMat;
    public Material hurtMat, invunMat;
    public SkinnedMeshRenderer meshRenderer, meshRendererTwo;
    
    private bool invun = false;
    public enum Phases
    {
        First,
        First_Ad_Phase,
        Second,
        Second_Ad_Phase,
        Third
    }

    private bool beenHealed;

    private Phases phase;
    
    
    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player");

        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 3.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(1), 3.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 3.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(1), 3.0f));


        anim = GetComponent<Animator>();
        phase = Phases.First;
    }

    /*
     * Takes damage and plays hurt effect
     */
    public override void TakeHealth(int damage)
    {
        if (invun) return;
        meshRenderer.material = hurtMat;
        meshRendererTwo.material = hurtMat;
        Wait(0.2f, () =>  meshRenderer.material = normalMat);
        Wait(0.2f, () =>  meshRendererTwo.material = normalMat);
        base.TakeHealth(damage);
    }

    //Starts the boss fight
    public void ActivateBoss()
    {
        if (!hasBeenActivated)
        {
            UseNextMove();
            hasBeenActivated = true;
        }
    }
    
    
    /**
     * Checks to see if health is low enough to spawn ads, in which case uses that move
     * else will pick a random move from the unlocked move set based on health
     */
    private void UseNextMove()
    {
        if (currentHealth < GetMaxHealth() * 0.8f && !FirstAdBool)
        {
            nextMoveQueue.Clear();
            meshRenderer.material = invunMat;
            meshRendererTwo.material = invunMat;
            invun = true;

            FirstAdBool = true;
            First_Ads.SetActive(true);
            phase = Phases.First_Ad_Phase;
        }
        if (currentHealth < GetMaxHealth() * 0.3f && !SecondAdBool)
        {
            nextMoveQueue.Clear();
            meshRenderer.material = invunMat;
            meshRendererTwo.material = invunMat;
            invun = true;
            Second_Ads.SetActive(true);
            SecondAdBool = true;
            phase = Phases.Second_Ad_Phase;
        }
        if (nextMoveQueue.Count < 1)
        {
            int x = Random.Range(0, 5) + 1;

            switch (phase)
            {
                case Phases.First:
                    switch (x)
                    {
                        case 1:
                            MoveSetOne();
                            break;
                        case 2:
                            MoveSetTwo();
                            break;
                        case 3:
                            MoveSetThree();
                            break;
                        case 4:
                            MoveSetFour();
                            break;
                        case 5:
                            MoveSetFive();
                            break;
                    }
                    
                    break;
                case Phases.Second:
                    switch (x)
                    {
                        case 1:
                            MoveSetSix();
                            break;
                        case 2:
                            MoveSetSeven();
                            break;
                        case 3:
                            MoveSetEight();
                            break;
                        case 4:
                            MoveSetNine();
                            break;
                        case 5:
                            MoveSetTen();
                            break;
                    }
                    break;
                case Phases.Third:
                    switch (x)
                    {
                        case 1:
                            MoveSetEleven();
                            break;
                        case 2:
                            MoveSetTwelve();
                            break;
                        case 3:
                            MoveSetThirteen();
                            break;
                        case 4:
                            MoveSetFourteen();
                            break;
                        case 5:
                            MoveSetFifteen();
                            break;
                    }
                    break;
                case Phases.First_Ad_Phase:
                    nextMoveQueue.Clear();
                    print("Still here 1st");
                    if (checkAds(AdObjectsFirst))
                    {
                        meshRenderer.material = normalMat;
                        meshRendererTwo.material = normalMat;

                        invun = false;

                        print("All dead 1st");
                        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(Random.Range(0,2), wallTimer), 10.0f));
                        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(Random.Range(0,2), wallTimer), 10.0f));
                        phase = Phases.Second;
                    }
                    else
                    {
                        nextMoveQueue.Enqueue(new MoveSetStruct(() => print("pass"), 0.0f));
                    }
                    
                    
                    break;
                case Phases.Second_Ad_Phase:
                    nextMoveQueue.Clear();
                    if (checkAds(AdObjectsSecond))
                    {
                        invun = false;
                        meshRenderer.material = normalMat;
                        meshRendererTwo.material = normalMat;
                        nextMoveQueue.Enqueue(new MoveSetStruct(() => lazerAttack(-0.1f), maxLazerTime));
                        phase = Phases.Second;
                    } else {nextMoveQueue.Enqueue(new MoveSetStruct(() => print("pass"), 0.0f)); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
            MoveSetStruct nextMove = nextMoveQueue.Dequeue();
            nextMove.moveAction();
            Wait(nextMove.timeTilNextMove, () => UseNextMove());
        
    }

    //Checks to see if any of the ADS are still alive in the boss room
    public bool checkAds(GameObject[] goList)
    {
        foreach (GameObject go in goList)
        {
            if (go) return false;
        }
        return true;
    }

    
    //simply used to check if the boss is dead
    private void Update()
    {
        
        
        if (IsDead())
        {
            phases--;
            if (phases < 1)
            {
                anim.SetTrigger("Death");
                Destroy(lazerGameObject);
                foreach (var VARIABLE in horizontalWallList)
                {
                    Destroy(VARIABLE);
                }
                Wait(5.0f, () => {Death();});
                return;
            } 
            LevelChanger.FadeToScene("Victory");
            anim.SetTrigger("Transition");
        }

        
    }

    //Destroys the boss object, can be expanded upon if needs be
    private void Death()
    {
        Destroy(this.gameObject);
    }
    
    /**
     * MoveSetX simply defines the moveset that the boss will use next
     */

    private void MoveSetOne()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(1, 0), timeBetweenLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(1, 1), timeBetweenLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(1, 0), timeBetweenLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(1, 1), timeBetweenLines));



    }
    private void MoveSetTwo()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));



    }
    private void MoveSetThree()
    {

        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(Random.Range(0,2)), 2.0f));


    }
    private void MoveSetFour()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), (timeBetweenLines * amountOfLines)/2));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 5.0f));


    }
    private void MoveSetFive()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(1), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.0f));


    }
    private void MoveSetSix()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(Random.Range(0,2), wallTimer), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, wallTimer), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 2.0f));
    }

    private void MoveSetSeven()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, wallTimer), 4.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(0, wallTimer), 4.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, wallTimer), 4.0f));
    }

    private void MoveSetEight()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, 0.0f), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));

    }

    private void MoveSetNine()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(0, 0.0f), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));
    }

    private void MoveSetTen()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), timeBetweenLines * amountOfLines));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.75f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.75f));
    }

    private void MoveSetEleven()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 0.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lazerAttack(-1.0f), maxLazerTime));
    }

    private void MoveSetTwelve()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 0.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lazerAttack(-1.0f), maxLazerTime + 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, 0.0f), 0.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(0, 0.0f), 4.0f));
    }

    private void MoveSetThirteen()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, 0.0f), 0.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(0, 0.0f), 4.0f));

    }

    private void MoveSetFourteen()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), (timeBetweenLines * amountOfLines) * 0.7f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => OrbAttack(), 4.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, 0.0f), 0.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => HorizontalWallAttack(0, 0.0f), 1.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), (timeBetweenLines * amountOfLines)));
    }

    private void MoveSetFifteen()
    {
        nextMoveQueue.Enqueue(new MoveSetStruct(() => VerticalWallAttack(0, 0.0f), 2.0f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => lineAttack(amountOfLines, 1), (timeBetweenLines * amountOfLines) * 0.7f));
        nextMoveQueue.Enqueue(new MoveSetStruct(() => SemiCircleShot(0), 3.0f));
    }
    
    
    
    /*
     * Unused code, old way of spawning bullets
     */
    IEnumerator bulletAttack(BulletStyles spawnStyle, int iter, int maxIter)
    {
        anim.SetTrigger("Attack1");
        List<GameObject> allBullets = new List<GameObject>();
        List<Vector3> pointsToSpawn = new List<Vector3>();

        float spawnInTimer = 0.0f; //Timer to spawn in the bullets
        float timeToFireTimer = 0.0f; //timer til they start firing
        float firingTimer = 0.0f; //timer inbetween bullet shots
        float iterTimer = 0.05f; //timer for after this one was fired.
        switch (spawnStyle)
        {
            case BulletStyles.Circular:
                for (float i = 0; i < 360; i += 360/amountOfBullets)
                {
                    i += amountOfBullets / ((iter % 2) + 1);
                    float outerX = bulletCircleRadius * (Mathf.Cos(Mathf.Deg2Rad * i ));
                    float outerZ = bulletCircleRadius * (Mathf.Sin(Mathf.Deg2Rad * i ));
                    pointsToSpawn.Add(new Vector3(outerX,0,outerZ));
                    i -= amountOfBullets / ((iter % 2) + 1);

                }

                timeToFireTimer = 0.6f;
                iterTimer = 0.4f;
                break;
            case BulletStyles.Spiral:
                for (float i = 0; i < 360; i += 360/amountOfBullets)
                {
                    i += amountOfBullets / ((iter % 2) + 1);

                    float outerX = bulletCircleRadius * (Mathf.Cos(Mathf.Deg2Rad * i ));
                    float outerZ = bulletCircleRadius * (Mathf.Sin(Mathf.Deg2Rad * i ));
                    pointsToSpawn.Add(new Vector3(outerX,0,outerZ));
                    i -= amountOfBullets / ((iter % 2) + 1);

                }
                iterTimer = 0.0f;
                firingTimer = 0.04f;
                break;
            case BulletStyles.Pentagram:

                //float[,] pentagramPoints = new float[4,4];
                Vector3[] outerPoints = new Vector3[5];
                for (float i = 0; i < 360; i += 360/amountOfBullets)
                {
                    float outerX = bulletCircleRadius * (Mathf.Cos(Mathf.Deg2Rad * i ));
                    float outerZ = bulletCircleRadius * (Mathf.Sin(Mathf.Deg2Rad * i ));
                    pointsToSpawn.Add(new Vector3(outerX,0,outerZ));
                }
                for (int i = 0; i < 5; i++)
                {
                    float outerX = bulletCircleRadius * (Mathf.Cos(Mathf.Deg2Rad * (90 + (i * (360 / 5)))));
                    float outerZ = bulletCircleRadius * (Mathf.Sin(Mathf.Deg2Rad * (90 + (i * (360 / 5)))));
                    Vector3 outer = new Vector3(outerX,0,outerZ);
                    outerPoints[i] = outer;
                }
                for (int i = 0; i < 5; i++)
                {
                    int x = (i * 2) % 5;
                    int y = (x + 2) % 5;
                    {
                        float dis = Vector3.Distance(outerPoints[y], outerPoints[x]);
                        Vector3 dir = Vector3.Normalize(outerPoints[y] - outerPoints[x]);
                        for (int j = 0; j < 10; j++)
                        {
                            {
                                Vector3 point = dir * (j * (dis / 10));
                                pointsToSpawn.Add(point + outerPoints[x]);
                            }
                        }
                    }
                }

                timeToFireTimer = 0.6f;
                spawnInTimer = 0.04f;
                iterTimer = 0.6f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(spawnStyle), spawnStyle, null);
        }
        
        foreach (var bulletLocation in pointsToSpawn)
        {
            Quaternion lookRot = Quaternion.LookRotation(bulletLocation);
            allBullets.Add(Instantiate(bullet, bulletLocation + transform.position, lookRot));
            if(spawnInTimer > 0) yield return new WaitForSeconds(spawnInTimer);
        }

        if(timeToFireTimer > 0) yield return new WaitForSeconds(timeToFireTimer);
        foreach (var bulletObj in allBullets)
        {
            if(!bulletObj) continue;
            bulletObj.GetComponent<ProjectileClass>().SetUp(1, true);
            bulletObj.GetComponent<Rigidbody>().AddForce(bulletObj.transform.forward * bulletSpeed);
            if(firingTimer > 0) yield return new WaitForSeconds(firingTimer);
        }
        iter++;

        if (IsDead())
        {
            foreach (var bulletGO in allBullets)
            {
                Destroy(bulletGO);
            }
        }
        //Wait(0.5f, () => bulletAttack(spawnStyle, iter, maxIter)); //Allows us to continue to spawn after a delay
        if(iterTimer > 0) yield return new WaitForSeconds(iterTimer);
        if (iter <= maxIter) StartCoroutine(bulletAttack(spawnStyle, iter, maxIter));
        if (spawnStyle == BulletStyles.Pentagram && iter == maxIter)
        {
            spawnBulletPentagramSFX.Play();
            spawnBulletPentagramSFX.time = 0.4f;
        }
    }


    void lineAttack(int linesLeft, int missInt)
    {
        float dis = Vector3.Distance(leftSpawnObject.transform.position, rightSpawnObject.transform.position);
        anim.SetTrigger("Attack1");
        for (int i = 0; i < amountOfLineBullets; i++)
        {
            if (i % 2 == missInt)
            {
                Vector3 spawnLoc = Vector3.Lerp(rightSpawnObject.transform.position, leftSpawnObject.transform.position, (float)i / (float)amountOfLineBullets);
                GameObject bulletObj = Instantiate(bullet, spawnLoc, Quaternion.LookRotation(spawnLoc));
                bulletObj.GetComponent<ProjectileClass>().SetUp(1, true);
                bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
            }
        }

        missInt = missInt == 1 ? 0 : 1;
        if (linesLeft > 0) Wait(timeBetweenLines, () => lineAttack(linesLeft - 1, missInt));
    }

    void OrbAttack()
    {
        for (int i = 0; i < amountOfOrbShots; i++)
        {
            GameObject bulletObj = Instantiate(bullet, orbSpawnLocation.transform.position, Quaternion.LookRotation(orbSpawnLocation.transform.position));
            bulletObj.GetComponent<ProjectileClass>().SetUp(1, true);
            Vector3 dir = Vector3.Normalize(player.transform.position - orbSpawnLocation.transform.position);
            dir.y = 0;
            Wait(timeBetweenOrbs * i, () => bulletObj.GetComponent<Rigidbody>().AddForce(orbDir() * (bulletSpeed * orbSpeedMult)));
        }
    }

    Vector3 orbDir()
    {
        Vector3 dir = Vector3.Normalize(player.transform.position - orbSpawnLocation.transform.position);
        dir.y = 0;
        return dir;
    }

    void SemiCircleShot(int missInt)
    {
        for (float i = 180; i < 360; i += 180/amountOfBullets)
        {
            //i += amountOfBullets / ((missInt % 2) + 1);

            float outerX = bulletCircleRadius * (Mathf.Cos(Mathf.Deg2Rad * i ));
            float outerZ = bulletCircleRadius * (Mathf.Sin(Mathf.Deg2Rad * i ));
           // i -= amountOfBullets / ((missInt % 2) + 1);
            Vector3 spawnLoc = new Vector3(outerX,0, outerZ) + transform.position;
            GameObject bulletObj = Instantiate(bullet, spawnLoc, Quaternion.LookRotation(orbSpawnLocation.transform.position));
            bulletObj.GetComponent<ProjectileClass>().SetUp(1, true);
            Wait(1.5f, () => bulletObj.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(bulletObj.transform.position - transform.position)* (bulletSpeed)));


        }
    }
    

    //Swings lazer beam prefab around
    //Can be modified to use different patterns at a later date
    private void lazerAttack(float prevTime)
    {
        if (IsDead())
        {
            Destroy(lazerGameObject);
            return;
        }
        anim.SetTrigger("Attack2");
        

        if (prevTime <= 0)
        {
            if(lazerGameObject) Destroy(lazerGameObject);
            lazerGameObject = Instantiate(lazerObjectPrefab, transform.position, Quaternion.identity);
            lazerAttackSFX.Play();
            prevTime = 0.01f;
        }
        
        if (prevTime > maxLazerTime)
        {
            lazerAttackSFX.Stop();
            Destroy(lazerGameObject);
            return;
        }
        prevTime += Time.deltaTime;
        float RotSpeed = prevTime % lazerRotSpeed;
        float percentRotSpeed = RotSpeed/lazerRotSpeed;
        float newRotY = 360 * percentRotSpeed;
        lazerGameObject.transform.rotation = Quaternion.Euler(new Vector3(0,newRotY,0));
        
        Wait(0, () => lazerAttack(prevTime));// lazerAttack(prevTime);
    }

    
    //Unused code
    //Functionality works but didn't fit theme
    //Simply spawns bombs the player has to avoid.
    private void BombAttack(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Transform playerLocation = player.transform;
            GameObject bombGameObject = Instantiate(bombPrefab, playerLocation.position, Quaternion.identity);
            Wait(bombSpawnTimer, () => {});
            bombGameObject.GetComponent<HaalrenBombScript>().Blowup(bombRadius, bombBlowupTimer,bombDamage);
        }
    }


    private void HorizontalWallAttack(int startInt, float waitTime)
    {
        anim.SetTrigger("Attack3");

        
       // wallList[startInt].GetComponent<MeshRenderer>().enabled = true; //change this to hit marker warning
        horizontalWallList[startInt].GetComponent<OnRollingThunderScript>().SetWarningActive(true);

        Wait(wallTimer, () => TurnWallDangerous(horizontalWallList[startInt],wallTimer));
        if (startInt + 2 < horizontalWallList.Count)
        {
            Wait(waitTime,(() => HorizontalWallAttack(startInt + 2, waitTime)));

        }
        
    }
    private void VerticalWallAttack(int startInt, float waitTime)
    {
        anim.SetTrigger("Attack3");

        
        // wallList[startInt].GetComponent<MeshRenderer>().enabled = true; //change this to hit marker warning
        verticalWallList[startInt].GetComponent<OnRollingThunderScript>().SetWarningActive(true);

        Wait(wallTimer, () => TurnWallDangerous(verticalWallList[startInt],wallTimer));
        if (startInt + 2 < verticalWallList.Count)
        {
            Wait(waitTime,(() => VerticalWallAttack(startInt + 2, waitTime)));

        }
        
    }

    public void TurnWallDangerous(GameObject index, float timer)
    {
        index.GetComponent<OnRollingThunderScript>().SetDangerActive(true);
        Wait(timer, () => { TurnWallOff(index);});
        
    }

    public void TurnWallOff(GameObject index)
    {
        index.GetComponent<OnRollingThunderScript>().SetDangerActive(false);
        index.GetComponent<OnRollingThunderScript>().SetWarningActive(false);
    }

    
    //Function to help sleep during bullet attacks & choosing new attacks
    public void Wait(float seconds, Action action){
        StartCoroutine(_wait(seconds, action));
    }
    IEnumerator _wait(float time, Action callback){
        yield return new WaitForSeconds(time);
        if(!IsDead()) callback();
    }
}