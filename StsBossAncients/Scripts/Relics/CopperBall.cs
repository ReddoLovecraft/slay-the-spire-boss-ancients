using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using Patchouib.Scrpits.Main;
using StsBossAncients.Scripts.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StsBossAncients.Scripts.Relics
{
	[Pool(typeof(SharedRelicPool))]
	public sealed class CopperBall : StsBossAncientsRelic, IRightCilckable
	{
		public override RelicRarity Rarity => RelicRarity.Ancient;
		public override bool HasUponPickupEffect => true;

		private List<SerializableCard> _stored = new List<SerializableCard>();

		protected override void DeepCloneFields()
		{
			base.DeepCloneFields();
			_stored = new List<SerializableCard>();
		}

		[SavedProperty]
		public List<SerializableCard> StoredCards
		{
			get => _stored;
			set
			{
				AssertMutable();
				_stored = value ?? new List<SerializableCard>();
			}
		}

		[SavedProperty]
		public bool UsedThisCombat
		{
			get => _usedThisCombat;
			set
			{
				AssertMutable();
				_usedThisCombat = value;
			}
		}
		private bool _usedThisCombat;

		public override Task BeforeCombatStart()
		{
			UsedThisCombat = false;
			return Task.CompletedTask;
		}

		public override async Task AfterObtained()
		{
			await PutInFlow();
		}

		public async Task OnRightClick(PlayerChoiceContext context)
		{
			if (UsedThisCombat)
			{
				return;
			}
			if (Owner.Creature.CombatState?.RunState.CurrentRoom is not CombatRoom)
			{
				return;
			}
			if (StoredCards.Count == 0)
			{
				return;
			}

			if (Owner.Creature.CombatState == null)
			{
				return;
			}

			List<CardModel> cards = new List<CardModel>();
			foreach (SerializableCard sc in StoredCards)
			{
				CardModel loaded = CardModel.FromSerializable(sc);
				loaded.Owner = Owner;
				CardModel combatCard = Owner.Creature.CombatState.CloneCard(loaded);
				cards.Add(combatCard);
			}
			UsedThisCombat = true;
			Flash();
			await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
		}

		public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
		{
			if (player != Owner)
			{
				return false;
			}
			bool hasPutIn = options.Any(o => o is CopperBallPutInOption);
			bool hasTakeOut = options.Any(o => o is CopperBallTakeOutOption);
			if (!hasPutIn && StoredCards.Count == 0)
			{
				options.Add(new CopperBallPutInOption(player));
			}
			if (StoredCards.Count > 0 && !hasTakeOut)
			{
				options.Add(new CopperBallTakeOutOption(player));
			}
			return true;
		}

		private async Task PutInFlow()
		{
			CardSelectorPrefs prefs = new CardSelectorPrefs(new LocString("relics", $"{Id.Entry}.selectionScreenPrompt"), 2, 2)
			{
				RequireManualConfirmation = true,
				Cancelable = false
			};

			List<CardModel> selected = (await CardSelectCmd.FromDeckGeneric(Owner, prefs, null, null)).ToList();
			if (selected.Count == 0)
			{
				return;
			}

			foreach (CardModel c in selected)
			{
				StoredCards.Add(c.ToSerializable());
			}
			await CardPileCmd.RemoveFromDeck(selected, showPreview: true);
		}

		private async Task TakeOutFlow()
		{
			if (StoredCards.Count == 0)
			{
				return;
			}
			List<CardModel> cards = new List<CardModel>();
			foreach (SerializableCard sc in StoredCards)
			{
				CardModel c = CardModel.FromSerializable(sc);
				if (!Owner.RunState.ContainsCard(c))
				{
					Owner.RunState.AddCard(c, Owner);
				}
				cards.Add(c);
			}
			StoredCards.Clear();
			foreach (CardModel c in cards)
			{
				await CardPileCmd.Add(c, PileType.Deck);
			}
		}

		private sealed class CopperBallPutInOption : RestSiteOption
		{
			public override string OptionId => "COPPER_BALL_PUT_IN";
			public CopperBallPutInOption(Player owner)
				: base(owner)
			{
			}

			public override async Task<bool> OnSelect()
			{
				CopperBall? relic = Owner.Relics.OfType<CopperBall>().FirstOrDefault();
				if (relic == null)
				{
					return false;
				}
				relic.Flash();
				await relic.PutInFlow();
				return true;
			}
		}

		private sealed class CopperBallTakeOutOption : RestSiteOption
		{
			public override string OptionId => "COPPER_BALL_TAKE_OUT";

			public CopperBallTakeOutOption(Player owner)
				: base(owner)
			{
			}

			public override async Task<bool> OnSelect()
			{
				CopperBall? relic = Owner.Relics.OfType<CopperBall>().FirstOrDefault();
				if (relic == null)
				{
					return false;
				}
				relic.Flash();
				await relic.TakeOutFlow();
				return true;
			}
		}
	}
}
