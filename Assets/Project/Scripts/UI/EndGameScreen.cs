﻿using System;
using Core;
using Dependencies;
using Extensions;
using Saves;
using UnityEngine;
using Weapon;

namespace UI
{
    public class EndGameScreen
    {
        private class MoneyTracker
        {
            private int currentMoney => DataSaveLoader.SerializableData.money.Value;
            private int startingMoney;
            
            public MoneyTracker()
            {
                GameEvents.GameStarted.Event += () => startingMoney = currentMoney;
            }

            public int GetCollectedMoneyOnRound()
            {
                return currentMoney - startingMoney;
            }
        }

        private Action onScreenInteractionEnd;
            
        private MoneyTracker moneyTracker;
        private UIDependencies dependencies;
        
        public EndGameScreen(UIDependencies uiDependencies)
        {
            moneyTracker = new MoneyTracker();
            dependencies = uiDependencies;
            
            uiDependencies.noThanksButton.onClick.AddListener(SkipAd);
            uiDependencies.viewAdButton.onClick.AddListener(ShowAd);
        }

        public void ShowWinScreen()
        {
            Show();
            
            dependencies.endGameIcon.sprite = dependencies.winIconSprite;
            onScreenInteractionEnd = GameEvents.GameEndedByWin.Invoke;

            dependencies.aboveEndGameIcon.text = "";
        }

        public void ShowLoseScreen()
        {
            Show();
            
            dependencies.endGameIcon.sprite = dependencies.loseIconSprite;
            onScreenInteractionEnd = GameEvents.GameEndedByLose.Invoke;
            
            dependencies.aboveEndGameIcon.text = "Ran out of ammo";
        }

        private void Show()
        {
            dependencies.collectedMoneyOnRoundMultiplyText.text = (moneyTracker.GetCollectedMoneyOnRound() * 3).ToString();
            dependencies.endGameCanvas.gameObject.SetActive(true);
            dependencies.crosshairCanvas.gameObject.SetActive(false);

            UnityEvents.Update += RotateGlow;
        }

        
        private void Hide()
        {
            dependencies.endGameCanvas.gameObject.SetActive(false);
            
            UnityEvents.Update -= RotateGlow;
        }

        private void RotateGlow()
        {
            dependencies.rotatableGlow.rectTransform.Rotate(Vector3.forward, dependencies.rotateGlowSpeed * Time.deltaTime);
        }

        private void ShowAd()
        {
            //reward
            DataSaveLoader.SerializableData.money.Value += moneyTracker.GetCollectedMoneyOnRound() * 3;
            Hide();
            onScreenInteractionEnd();
        }

        private void SkipAd()
        {
            //inter
            Hide();
            onScreenInteractionEnd();
        }
    }
}