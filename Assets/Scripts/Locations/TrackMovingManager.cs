using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// moving tracks on the location map animatedly
[RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
public class TrackMovingManager : MonoBehaviour
{
    public List<Animator> mapTracks; // list of all map tracks

    public ScrollRect scrollRect; // scroll rect

    public List<Animator> mapItemsAnimators; // list of animated items's animators(3)

    public void Start()
    {
        // add valueChanged event listener
        scrollRect.onValueChanged.AddListener(UpdateTrack);

        // play start animation
        StartCoroutine(StartTrackAnimation());
    }
    private void UpdateTrack(Vector2 value)
    {
        // *** OLD *** [first thoughts]
        // print(scrollRect.verticalNormalizedPosition);
        // normalizedPosition = 0 - 1
        // if tracks amount is 28
        // and 7/28 = 0 and 28/28 = 1
        // then 21 left(it means normalizedPosition * 21 is needed value)
        // 0.3 * 21 = 6(3) ~ 6(tracks from the beginning need to be shown)
        // const int availableTracksAmount = 21;
        // float verNormalizedPosition = scrollRect.verticalNormalizedPosition;
        // int tracksAmount = (int)(availableTracksAmount * verNormalizedPosition);

        int availableTracksAmount = mapTracks.Count; // available tracks amount of current track line(28)
        float verNormalizedPosition = scrollRect.verticalNormalizedPosition; // get specific value of normalized position

        int tracksAmount = (int)(availableTracksAmount * verNormalizedPosition); // get amount of tracks to show

        if (verNormalizedPosition > 0)
            // multiply amount of track with calculated value(self made function)
            tracksAmount += 7 - (int)(7 * ((float)tracksAmount / 28));
        else
            // just add 7 (always visible) points [manually calculated]
            tracksAmount += 7;

        // check and control every part of tracks
        for (int i = 0; i < availableTracksAmount; i++)
        {
            if (tracksAmount > i)
            {
                // show piece of track
                mapTracks[i].SetBool("Showed", true);

                // show sign
                if (i == 11) mapItemsAnimators.Find(t => t.name == "Sign").SetBool("Appearing", true);
                if (i == 22)
                {
                    mapItemsAnimators.Find(t => t.name == "Pistol").SetBool("Falling", true);
                    mapItemsAnimators.Find(t => t.name == "Holes").SetBool("Shooting", true);
                } // if
            }
            else
            {
                // hide piece of track
                mapTracks[i].SetBool("Showed", false);
            }
        } // for i
    }
    private IEnumerator StartTrackAnimation()
    {
        // TODO: start animation of first 7 pieces of track
        for (int i = 0; i < 7; i++)
        {
            mapTracks[i].SetBool("Showed", true);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
