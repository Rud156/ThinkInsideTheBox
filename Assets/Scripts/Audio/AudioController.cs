using UnityEngine;
using Random = UnityEngine.Random;


namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        [Header("Prefabs")] public GameObject singleShotAudio;

        [Header("Sounds")] public Sound background;
        public Sound gearClick;
        public Sound[] gearTurn;

        #region External Functions

        public void PlaySound(AudioEnum audioEnum)
        {
            GameObject singleShotAudioInstance = Instantiate(singleShotAudio, transform, true);
            AudioSource audioSource = singleShotAudioInstance.GetComponent<AudioSource>();

            switch (audioEnum)
            {
                case AudioEnum.Background:
                {
                    audioSource.clip = background.clip;
                    audioSource.volume = background.volume;
                    audioSource.pitch = background.pitch;
                    audioSource.loop = background.loop;
                }
                    break;

                case AudioEnum.GearClick:
                {
                    audioSource.clip = gearClick.clip;
                    audioSource.volume = gearClick.volume;
                    audioSource.pitch = gearClick.pitch;
                    audioSource.loop = gearClick.loop;
                }
                    break;

                case AudioEnum.GearTurning:
                {
                    Sound randomTurnSound = gearTurn[Mathf.FloorToInt(Random.value * gearTurn.Length)];
                    audioSource.clip = randomTurnSound.clip;
                    audioSource.volume = randomTurnSound.volume;
                    audioSource.pitch = randomTurnSound.pitch;
                    audioSource.loop = randomTurnSound.loop;
                }
                    break;
            }

            audioSource.Play();
        }

        #endregion

        #region Enums

        public enum AudioEnum
        {
            Background,
            GearClick,
            GearTurning
        }

        #endregion

        #region Singleton

        private static AudioController _instance;
        public static AudioController Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion
    }
}