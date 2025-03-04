using Script.EventBus;
using UnityEngine;

namespace Script.manager
{
    public class AudioManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM Audio Clip")] 
        [SerializeField] private AudioClip mainMenuBgm;
        [SerializeField] private AudioClip inGameBgm;
        
        [Header("SFX Audio Clip")]
        [SerializeField] private AudioClip blockLockSfx;
        [SerializeField] private AudioClip blockMoveSfx;
        [SerializeField] private AudioClip blockRotateSfx;
        [SerializeField] private AudioClip blockStoredSfx;
        [SerializeField] private AudioClip lineClearSfx;
        
        private EventBindings<OnBlockMoved> _onBlockMoved;
        private EventBindings<OnBlockStored> _onBlockStored;
        private EventBindings<OnBlockRotated> _onBlockRotated;
        private EventBindings<OnBlockReachBottomEvent> _onBlockLocked;
        private EventBindings<OnDestroyRow> _onDestroyRow;

        private void Awake()
        {
            _onBlockMoved = new EventBindings<OnBlockMoved>(PlayBlockMoveSfx);
            _onBlockStored = new EventBindings<OnBlockStored>(PlayBlockStoredSfx);
            _onBlockRotated = new EventBindings<OnBlockRotated>(PlayBlockRotateSfx);
            _onBlockLocked = new EventBindings<OnBlockReachBottomEvent>(PlayBlockLockSfx);
            _onDestroyRow = new EventBindings<OnDestroyRow>(PlayLineClearSfx);
        }
        
        private void OnEnable()
        {
            Bus<OnBlockMoved>.Register(_onBlockMoved);
            Bus<OnBlockStored>.Register(_onBlockStored);
            Bus<OnBlockRotated>.Register(_onBlockRotated);
            Bus<OnBlockReachBottomEvent>.Register(_onBlockLocked);
            Bus<OnDestroyRow>.Register(_onDestroyRow);
        }
        
        private void OnDisable()
        {
            Bus<OnBlockMoved>.Unregister(_onBlockMoved);
            Bus<OnBlockStored>.Unregister(_onBlockStored);
            Bus<OnBlockRotated>.Unregister(_onBlockRotated);
            Bus<OnBlockReachBottomEvent>.Unregister(_onBlockLocked);
            Bus<OnDestroyRow>.Unregister(_onDestroyRow);
        }

        #region SFX

        private void PlayBlockMoveSfx()
        {
            sfxSource.PlayOneShot(blockMoveSfx);
        }
        
        private void PlayBlockRotateSfx()
        {
            sfxSource.PlayOneShot(blockRotateSfx);
        }
        
        private void PlayBlockLockSfx()
        {
            sfxSource.PlayOneShot(blockLockSfx);
        }
        
        private void PlayBlockStoredSfx()
        {
            sfxSource.PlayOneShot(blockStoredSfx);
        }
        
        private void PlayLineClearSfx()
        {
            sfxSource.PlayOneShot(lineClearSfx);
        }


        #endregion
    }
}
