using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// moving tracks on the location map animatedly
[RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
public class TrackMovingController : MonoBehaviour
{
    public List<GameObject> mapTracks; // list of all map tracks

    private ScrollRect scrollRect; // scroll rect

    public void Start()
    {
        // get scroll rect
        scrollRect = GetComponent<ScrollRect>();

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
                mapTracks[i].GetComponent<Animator>().SetBool("Showed", true);

                // show sign
                if (i == 11) GameObject.Find("Content/Map/Sign").GetComponent<Animator>().SetBool("Appearing", true);
                if (i == 22)
                {
                    GameObject.Find("Content/Map/Pistol").GetComponent<Animator>().SetBool("Falling", true);
                    GameObject.Find("Content/Map/Holes").GetComponent<Animator>().SetBool("Shooting", true);
                }
            }
            else
                // hide piece of track
                mapTracks[i].GetComponent<Animator>().SetBool("Showed", false);

            
        } // for i
    }
    private IEnumerator StartTrackAnimation()
    {
        // TODO: start animation of first 7 pieces of track
        for (int i = 0; i < 7; i++)
        {
            mapTracks[i].GetComponent<Animator>().SetBool("Showed", true);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
