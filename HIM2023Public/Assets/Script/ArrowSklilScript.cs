using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSklilScript : MonoBehaviour
{
    private float SkillTimer;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        SkillTimer = 0f;
        audioSource = GetComponent<AudioSource>();
    }

    Coroutine skillSound;
    void Update()
    {
        if (skillSound == null)
            skillSound = StartCoroutine(delaySoundPlay());
        SkillTimer += Time.deltaTime;

        if (SkillTimer >= 2f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("파티클 아무튼 닿음");

        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 닿음");
            LivingEntity _player = other.GetComponent<LivingEntity>();
            _player.TakeDamege(10f);
        }
    }
    IEnumerator delaySoundPlay()
    {
        while (true)
        {
            float range = Random.Range(0.1f, 0.3f);
            yield return new WaitForSecondsRealtime(range);
            //audioSource.PlayOneShot(audioSource.clip);

            GameObject openSound = new GameObject("Meteor Sound");
            openSound.transform.position = gameObject.transform.position;
            TemporarySoundPlayer soundPlayer = openSound.AddComponent<TemporarySoundPlayer>();
            soundPlayer.SoundPlay(audioSource.clip);
        }

    }
}