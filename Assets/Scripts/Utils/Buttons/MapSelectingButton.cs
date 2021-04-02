using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectingButton : ToggleButton
{
    public MapInfo currentMap; // current map info

    public TrackMovingManager trackMovingManager; // current map TrackMovingManager

    protected override void Perform()
    {
        // check for moving of scroll rect
        if (downPosition != upPosition)
            return;

        // reset vertical normalized position
        if (trackMovingManager.scrollRect.verticalNormalizedPosition != 0)
            trackMovingManager.scrollRect.verticalNormalizedPosition = 0;

        // reset map tracks
        foreach (Animator track in trackMovingManager.mapTracks)
        {
            // animator не работает, когда объект спрятан, поэтому вручную меняем прозрачность
            Image _image = track.GetComponent<Image>();
            Color _color = _image.color;

            if (_color.a != 0)
            {
                _color.a = 0;
                _image.color = _color;
            }
        }

        // select current map
        PlayerManager.instance.currentMap = currentMap;

        // toggle button init
        base.Perform();
    }
}
