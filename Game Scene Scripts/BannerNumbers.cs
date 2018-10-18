using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerNumbers : MonoBehaviour {
	private GameObject platformBannerNumbers, gapTrophyCovers;
	
	void Start () {
		if (gameObject.name == "BannerTL") {
			platformBannerNumbers = GameObject.Find ("ArchNumber");
			if (platformBannerNumbers.activeInHierarchy) {
				platformBannerNumbers.SetActive (false);
			}
			GameObject.Find ("PlatformBannersArch").SetActive (false);
		}else if(gameObject.name == "GapBannerL"){
			gapTrophyCovers = GameObject.Find ("GapBannerTrophy");
			gapTrophyCovers.SetActive (false);
		}
	}

	//Cover the banner number with a better quality number image
	public void turnOnBannerNumbers(){
		if (gameObject.name == "BannerTL") {
			if (!platformBannerNumbers.activeInHierarchy) {
				platformBannerNumbers.SetActive (true);
			}
		}
	}

	//Cover the banner with a better quality trophy image
	public void turnOnBannerTrophy(){
		if(gameObject.name == "GapBannerL"){
			if (!gapTrophyCovers.activeInHierarchy) {
				gapTrophyCovers.SetActive (true);
			}
		}
	}
}
