using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Orphan
{
	public class DialogueCollection
	{
		public static Transform parameterDialogueCharacter;

		public static string getCharacterName(DialogueCharacter character)
		{
			string s = "";
			switch (character)
			{
				case DialogueCharacter.None:
					break;
				case DialogueCharacter.Player:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "You:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Sen:";
					}
					break;
				case DialogueCharacter.OldMan:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Old Man:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Yaşlı Adam:";
					}
					break;
				case DialogueCharacter.Roommate:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Roommate:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Oda Arkadaşı:";
					}
					break;
				case DialogueCharacter.Nurse:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Nurse:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Hemşire:";
					}
					break;
				case DialogueCharacter.Principal:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Principal:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Müdür:";
					}
					break;
				case DialogueCharacter.QuestionMark:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "???:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "???:";
					}
					break;
				case DialogueCharacter.Mother:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Mother:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Anne:";
					}
					break;
				case DialogueCharacter.Father:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Father:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Baba:";
					}
					break;
				case DialogueCharacter.Smoker:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Smoker:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Sigaracı:";
					}
					break;
				case DialogueCharacter.Ghost:
					if (GameManagerScript.lang == Language.Eng)
					{
						s = "Ghost:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						s = "Hayalet:";
					}
					break;
				default:
					break;
			}
			return s;
		}

		public static DialogueNode getDialogueTree(int id)
		{
			int startNodeId = 0;
			switch (id)
			{
				case 1: //prologue old man 1
					startNodeId = 1;
					break;
				case 2: //prologue old man 2
					startNodeId = 3;
					break;
				case 3: //prologue old man 3
					startNodeId = 9;
					break;
				case 4: //prologue old man 4
					startNodeId = 16;
					break;
				case 5: //DAY 1 roommate 1
					startNodeId = 25;
					break;
				case 6: //DAY 1 roommate 2
					startNodeId = 36;
					break;
				case 7: //DAY 1 nurse 1
					startNodeId = 38;
					break;
				case 8: //DAY 1 nurse 2
					startNodeId = 68;
					break;
				case 9: //DAY 1 nurse 3
					startNodeId = 77;
					break;
				case 10: //DAY 1 principal 1
					startNodeId = 79;
					break;
				case 11: //DAY 1 principal 2
					startNodeId = 111;
					break;
				case 12: //DAY 1 principal 3
					startNodeId = 113;
					break;
				case 13: //ghost1
					startNodeId = 114;
					break;
				case 14: //DAY 1 nurse 4
					startNodeId = 115;
					break;
				default:
					break;
			}
			return getDialogueNode(startNodeId);
		}

		public static DialogueNode getDialogueNode(int id)
		{
			DialogueNodeType type = DialogueNodeType.QuestionNodeWithAnswers;
			string text = "";
			List<int> children = new List<int>();
			DialogueCharacter character = DialogueCharacter.None;

			switch (id)
			{
				case 1:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You need to find your parents.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne ve babanı bulmalısın.";
					}
					children.Add(2);
					character = DialogueCharacter.OldMan;
					break;
				case 2:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You've been dreaming for a while now.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bir süredir rüya görüyorsun.";
					}
					children.Add(3);
					character = DialogueCharacter.OldMan;
					break;
				case 3:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Dreams are precious. But what you really need is your parents.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Rüya görmek çok değerli. Ancak senin asıl ihtiyacın anne ve baban.";
					}
					character = DialogueCharacter.OldMan;
					children.Add(4);
					break;
				case 4:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Stop dreaming. Go find your parents. Now!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Rüya görmeyi bırak. Git anne babanı bul. Şimdi!";
					}
					character = DialogueCharacter.OldMan;
					break;
				case 5:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Everyone thinks the exact same thing before they go to sleep.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Herkes uyumadan önce aynı şeyi düşünür.";
					}
					character = DialogueCharacter.OldMan;
					children.Add(6);
					break;
				case 6:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "That they are cold and they miss mama?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Üşüyüp annelerini özlediklerini mi?";
					}
					character = 0;
					children.Add(7);
					break;
				case 7:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Yes, exactly! I wouldn't be able to put it in better words.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Evet, aynen öyle! Daha iyi ifade edilemezdi.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (8);
					break;
				case 8:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "That's odd.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Çok garip.";
					}
					character = 0;
					break;
				case 9:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "This is your room. This belongs to you. Do you remember?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burası senin odan. Burası sana ait. Hatırlıyor musun?";
					}
					character = DialogueCharacter.OldMan;
					children.Add (12);
					children.Add (13);
					break;
				case 10:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You played games here. You were safe.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burada oyunlar oynardın. Burada güvendeydin.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (11);
					break;
				case 11:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Not anymore. Find your parents and you'll be safe again.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ama artık değil. Anne babanı bul, yine güvende olacaksın.";
					}
					character = DialogueCharacter.OldMan;
					break;
				case 12:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Yes.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Evet.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (14);
					break;
				case 13:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "No.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hayır.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (15);
					break;
				case 14:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "That's good to hear.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sevindim.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (10);
					break;
				case 15:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Oh, my poor child!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ah, zavallı evladım!";
					}
					character = DialogueCharacter.OldMan;
					children.Add (10);
					break;
				case 16:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'll let you on this spaceship if you find your parents.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Eğer anne babanı bulursan bu uzay gemisine binmene izin vereceğim.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (17);
					children.Add (18);
					break;
				case 17:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Why can't I get in now?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Neden şimdi binemiyorum?";
					}
					character = DialogueCharacter.Player;
					children.Add (19);
					break;
				case 18:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "That's not fair!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç adil değil!";
					}
					character = DialogueCharacter.Player;
					children.Add (22);
					break;
				case 19:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "My child, I'm afraid some things can only be done given the sufficient time.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Çocuğum, korkarım ki bazı şeyler sadece yeterli zaman verildiğinde yapılabilir.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (20);
					break;
				case 20:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I have to wait?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Beklemek mi zorundayım?";
					}
					character = DialogueCharacter.Player;
					children.Add (21);
					break;
				case 21:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "No, you just have to go through life.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hayır, sadece daha önce bir şeyler yaşamalısın.";
					}
					character = DialogueCharacter.OldMan;
					break;
				case 22:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "If we could decide what's fair and not, we would not be here.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Eğer neyin adil olup olmadığına biz karar verseydik şu an burada olmazdık.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (23);
					break;
				case 23:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "We are only the explorers of our own stories.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Biz sadece kendi hayatlarımızı keşfederiz.";
					}
					character = DialogueCharacter.OldMan;
					children.Add (24);
					break;
				case 24:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Go after your destiny, find your parents. Then the spaceship will be yours.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Kaderinin peşinden git, anne babanı bul. Sonra uzay gemisi senin olacak.";
					}
					character = DialogueCharacter.OldMan;
					break;
				case 25:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Hey! You slept for a very long time! Are you okay?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Merhaba! Ne kadar fazla uyudun öyle! İyi misin?";
					}
					character = DialogueCharacter.Roommate;
					children.Add (26);
					children.Add (27);
					break;
				case 26:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm fine, thanks.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "İyiyim, sağol.";
					}
					character = DialogueCharacter.Player;
					children.Add (28);
					break;
				case 27:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Who are you?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sen kimsin?";
					}
					character = DialogueCharacter.Player;
					children.Add (29);
					break;
				case 28:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Good to hear that.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bunu duyduğuma sevindim.";
					}
					character = DialogueCharacter.Roommate;
					children.Add (36);
					break;
				case 29:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "It's that bad, huh? You don't know me anymore?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Durum o kadar kötü ha? Artık beni bile hatırlamıyor musun?";
					}
					character = DialogueCharacter.Roommate;
					children.Add (30);
					break;
				case 30:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm your roommate. I've been helping you with your condition for a while.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ben senin oda arkadaşınım. Bir süredir senin bakımına yardım ediyorum.";
					}
					character = DialogueCharacter.Roommate;
					children.Add (31);
					children.Add (32);
					children.Add (33);
					break;
				case 31:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "What condition?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Neyin bakımı?";
					}
					character = DialogueCharacter.Player;
					children.Add (35);
					break;
				case 32:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Where are we?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Biz neredeyiz?";
					}
					character = DialogueCharacter.Player;
					children.Add (35);
					break;
				case 33:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You're annoying me.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Canımı sıkıyorsun.";
					}
					character = DialogueCharacter.Player;
					children.Add (34);
					break;
				case 34:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Okay, sorry. See you later.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Pardon, tamam. Sonra görüşürüz.";
					}
					character = DialogueCharacter.Roommate;
					children.Add (36);
					break;
				case 35:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You don't remember anything? You should go see the nurse or the principal.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç bir şey hatırlamıyor musun? Bence hemşireyi ya da müdürü ziyaret etmelisin.";
					}
					character = DialogueCharacter.Roommate;
					children.Add (36);
					break;
				case 36:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Please, go see the nurse. She will help you. She is at the lower floor.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Lütfen, hemşireyi ziyaret et. O sana yardım eder. Aşağı katta.";
					}
					character = DialogueCharacter.Roommate;
					children.Add (37);
					break;
				case 37:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You can use the stairs on the north when you leave this room to go to the nurse.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Odadan çıkınca kuzeydeki merdivenleri kullanarak hemşireye gidebilirsin.";
					}
					character = DialogueCharacter.Roommate;
					break;
				case 38:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Welcome dear. How are you today?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hoşgeldin oğlum. Bugün nasılsın?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (39);
					children.Add (40);
					break;
				case 39:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm fine, I guess.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bilmem, herhalde iyiyim.";
					}
					character = DialogueCharacter.Player;
					children.Add (58);
					break;
				case 40:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I don't remember anything!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç bir şey hatırlamıyorum!";
					}
					character = DialogueCharacter.Player;
					children.Add (41);
					break;
				case 41:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Nothing?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç bir şey mi?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (42);
					children.Add (43);
					break;
				case 42:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Nothing.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç bir şey.";
					}
					character = DialogueCharacter.Player;
					children.Add (51);
					break;
				case 43:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Where are my parents?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne babam nerede?";
					}
					character = DialogueCharacter.Player;
					children.Add (44);
					break;
				case 44:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "My poor boy! Your condition is getting worse and worse.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Zavallı oğlum! Gittikçe durumun kötüleşiyor.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (45);
					break;
				case 45:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I don't know how to tell you this.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bunu nasıl diyeceğimi bilmiyorum.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (46);
					break;
				case 46:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You are an orphan. You don't have parents. My poor boy! I am so sorry.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sen yetimsin. Anne baban yok. Zavallı oğlum! Çok üzgünüm.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (47);
					break;
				case 47:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "But, right now, we should be concerned about your health.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ama şu anda önem vermemiz gereken şey senin sağlığın.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (48);
					break;
				case 48:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I wish I could cure you right away. I really do. But for now this is all I can do.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Keşke seni hemen iyileştirebilseydim. Ancak şimdilik elimden sadece bu kadarı geliyor.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (49);
					break;
				case 49:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "But that's enough sad news for today! I have to give you your daily medicine.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ama bugünlük bu kadar kötü haber yeter! Sana günlük ilacını vermeliyim.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (50);
					break;
				case 50:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Are you ready? Just swallow it with water.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hazır mısın? Suyla beraber yut gitsin.";
					}
					character = DialogueCharacter.Nurse;
					break;
				case 51:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "My poor boy! Ah, my little poor boy!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Zavallı oğlum! Ah benim zavalı küçük oğlum!";
					}
					character = DialogueCharacter.Nurse;
					children.Add (52);
					break;
				case 52:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Maybe it was your destiny! You forgot the things that cause you pain. Maybe that's for the better.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Belki de kaderin böyleymiş! Sana acı veren şeyleri unutuverdin. Belki böylesi daha iyidir.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (53);
					children.Add (54);
					break;
				case 53:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm scared.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Korkuyorum.";
					}
					character = DialogueCharacter.Player;
					children.Add (55);
					break;
				case 54:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Where are my parents?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne babam nerede?";
					}
					character = DialogueCharacter.Player;
					children.Add (45);
					break;
				case 55:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Ah, I shouldn't be scaring a small child like you! I'm so sorry dear!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ah, senin gibi küçük bir çocuğu böyle korkutmamalıyım! Çok özür dilerim yavrum!";
					}
					character = DialogueCharacter.Nurse;
					children.Add (56);
					break;
				case 56:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm such an inconsiderate woman!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Çok düşüncesiz bir kadınım!";
					}
					character = DialogueCharacter.Nurse;
					children.Add (57);
					break;
				case 57:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "It's just... I feel so sad about you.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sadece... Sana çok üzülüyorum.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (49);
					break;
				case 58:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "My sweet little boy.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Tatlı oğlum benim.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (59);
					break;
				case 59:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "God help your innocent soul. You're just a little child!";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Allah yardımcın olsun. Sen daha küçücük masum bir çocuksun!";
					}
					character = DialogueCharacter.Nurse;
					children.Add (60);
					children.Add (61);
					break;
				case 60:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Do you know where my parents are?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne babamın nerede olduğunu biliyor musun?";
					}
					character = DialogueCharacter.Player;
					children.Add (63);
					break;
				case 61:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Can I get my medicine?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "İlacımı alabilir miyim?";
					}
					character = DialogueCharacter.Player;
					children.Add (62);
					break;
				case 62:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Of course, honey. Here comes your medicine.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Tabi ki, yavrum. İlacını hemen getiriyorum.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (50);
					break;
				case 63:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Your parents? Honey, you know where we are right now, right?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne baban mı? Canım, şu anda nerede olduğumuzu biliyorsun, değil mi?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (64);
					break;
				case 64:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "This is an orphanage, my child.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burası bir yetimhane, yavrum.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (65);
					break;
				case 65:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I know you can't help it, but please try not to think about your parents.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Biliyorum, elinde değil, ama lütfen anne babanı düşünmemeye çalış.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (66);
					break;
				case 66:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You are sick. Thinking about these is bad for your health.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sen hastasın. Bunları düşünmek sağlığına kötü gelir.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (67);
					break;
				case 67:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Anyway, honey. Let's give you your daily medicine, shall we?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Neyse canım. Hadi sana günlük ilacını verelim.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (50);
					break;
				case 68:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "That wasn't so bad, was it?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Çok da zor değildi, değil mi?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (69);
					break;
				case 69:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Do you need anything else, my child?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Başka bir şeye ihtiyacın var mı yavrum?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (70);
					children.Add (71);
					break;
				case 70:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I need to know more about myself.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Kendim hakkında daha fazla şey bilmem lazım.";
					}
					character = DialogueCharacter.Player;
					children.Add (72);
					break;
				case 71:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm searching for my parents. Can you help?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ben anne babamı arıyorum. Yardım eder misin?";
					}
					character = DialogueCharacter.Player;
					children.Add (72);
					break;
				case 72:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Honey, you are worrying me. You need to be resting. You are sick.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Yavrum, beni endişelendiriyorsun. Senin dinlenmen gerekiyor, sen hastasın.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (73);
					break;
				case 73:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Please help.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Lütfen yardım et.";
					}
					character = DialogueCharacter.Player;
					children.Add (74);
					break;
				case 74:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I really think this is bad for your health.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bunun gerçekten sağlığına kötü geleceğini düşünüyorum.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (75);
					break;
				case 75:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "But I will tell you this:";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ama sana şunu söyleyeyim:";
					}
					character = DialogueCharacter.Nurse;
					children.Add (76);
					break;
				case 76:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "They don't give me information about the kids staying here.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bana burada kalan çocuklar hakkında bilgi verilmiyor.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (77);
					break;
				case 77:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "The principal is responsible for everything around here. You might want to go to him.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Buralardaki her şeyden müdür sorumlu. Ona gitmek isteyebilirsin.";
					}
					character = DialogueCharacter.Nurse;
					children.Add (78);
					break;
				case 78:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "His room is on the upper floor, the middle one of the rooms to the north.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Odası üst katta, kuzeydeki odaların ortada olanı.";
					}
					character = DialogueCharacter.Nurse;
					break;
				case 79:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Hello?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Merhaba?";
					}
					character = DialogueCharacter.Principal;
					children.Add (80);
					break;
				case 80:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Are you the principal?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Müdür siz misiniz?";
					}
					character = DialogueCharacter.Player;
					children.Add (81);
					break;
				case 81:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Yes, that is me. What do you want?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Evet, benim. Ne istiyorsun?";
					}
					character = DialogueCharacter.Principal;
					children.Add (82);
					children.Add (83);
					break;
				case 82:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I have a problem.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bir sorunum var.";
					}
					character = DialogueCharacter.Player;
					children.Add (84);
					break;
				case 83:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I have questions I want to ask.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sormak istediğim sorularım var.";
					}
					character = DialogueCharacter.Player;
					children.Add (102);
					break;
				case 84:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "What is it?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Neymiş sorunun?";
					}
					character = DialogueCharacter.Principal;
					children.Add (85);
					children.Add (86);
					break;
				case 85:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I can't remember anything.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hiç bir şey hatırlayamıyorum.";
					}
					character = DialogueCharacter.Player;
					children.Add (87);
					break;
				case 86:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I'm not supposed to be here.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burada olmamalıyım.";
					}
					character = DialogueCharacter.Player;
					children.Add (99);
					break;
				case 87:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Look kid. I'm a busy man.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bak çocuk. Ben meşgul biriyim.";
					}
					character = DialogueCharacter.Principal;
					children.Add (88);
					break;
				case 88:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I know you're having health issues.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sağlık sorunların olduğunu biliyorum.";
					}
					character = DialogueCharacter.Principal;
					children.Add (89);
					break;
				case 89:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I didn't know you were losing your memory. That's not good.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hafızanı kaybetmeye başladığını bilmiyordum. Bu iyi değil.";
					}
					character = DialogueCharacter.Principal;
					children.Add (90);
					break;
				case 90:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "There are a lot of children in this orphanage. All of these have problems.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bu yetimhanede bir sürü çocuk kalıyor. Bunların hepsinin de sorunları var.";
					}
					character = DialogueCharacter.Principal;
					children.Add (91);
					break;
				case 91:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Don't act like you're the special one among all these children.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sanki bu çocuklardan en özeli senmişsin gibi davranmayı bırak.";
					}
					character = DialogueCharacter.Principal;
					children.Add (92);
					break;
				case 92:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Go back to your room and think about this, OK?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Odana geri dön ve bunları düşün, tamam mı?";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					children.Add (94);
					break;
				case 93:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "OK.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Tamam.";
					}
					character = DialogueCharacter.Player;
					break;
				case 94:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "But the nurse said...";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ama hemşire dedi ki...";
					}
					character = DialogueCharacter.Player;
					children.Add (95);
					break;
				case 95:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "What did she say?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ne dedi?";
					}
					character = DialogueCharacter.Principal;
					children.Add (96);
					children.Add (97);
					break;
				case 96:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "She said you would help.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Senin yardım edeceğini söyledi.";
					}
					character = DialogueCharacter.Player;
					children.Add (98);
					break;
				case 97:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "She said she can't help me in these conditions.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bu koşullarda beni iyileştiremeyeceğini söyledi.";
					}
					character = DialogueCharacter.Player;
					children.Add (98);
					break;
				case 98:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "There's nothing I can do.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Yapabileceğim bir şey yok.";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					break;
				case 99:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "What the hell are you talking about?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ne saçmalıyorsun sen şimdi?";
					}
					character = DialogueCharacter.Principal;
					children.Add (100);
					break;
				case 100:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I don't feel I belong here.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Buraya ait gibi hissetmiyorum.";
					}
					character = DialogueCharacter.Player;
					children.Add (101);
					break;
				case 101:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You're saying stupid things. Go to your room. This talk is over.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Aptal aptal konuşuyorsun. Odana git. Bu konuşma bitmiştir.";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					break;
				case 102:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You have questions? What kind of questions?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Soruların mı var? Nasıl sorular?";
					}
					character = DialogueCharacter.Principal;
					children.Add (103);
					children.Add (104);
					break;
				case 103:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Where are my parents?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Anne babam nerede?";
					}
					character = DialogueCharacter.Player;
					children.Add (105);
					break;
				case 104:
					type = DialogueNodeType.AnswerNode;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Can I get out of this building?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bu binadan dışarı çıkabilir miyim?";
					}
					character = DialogueCharacter.Player;
					children.Add (108);
					break;
				case 105:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You do realize this is an orphanage, right?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burasının bir yetimhane olduğunun farkındasın, değil mi?";
					}
					character = DialogueCharacter.Principal;
					children.Add (106);
					break;
				case 106:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "You are an orphan. You don't have parents.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Sen yetimsin. Senin anne baban yok.";
					}
					character = DialogueCharacter.Principal;
					children.Add (107);
					break;
				case 107:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Your sickness is getting to you. Go to your room and rest.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Hastalığın aklına vurmuş. Şimdi odana git ve dinlen.";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					break;
				case 108:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Why? There's nothing outside.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Neden? Dışarıda hiç bir şey yok.";
					}
					character = DialogueCharacter.Principal;
					children.Add (109);
					break;
				case 109:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "This is your home. You better get used to it.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burası senin evin. Alışsan iyi edersin.";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					break;
				case 110:
					type = DialogueNodeType.QuestionNodeWithAnswers;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "This is your home. You better get used to it.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Burası senin evin. Alışsan iyi edersin.";
					}
					character = DialogueCharacter.Principal;
					children.Add (93);
					break;
				case 111:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Don't cry in front of me! You're a man! You shouldn't cry.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Önümde ağlama! Kocaman adamsın sen! Ağlamamalısın.";
					}
					character = DialogueCharacter.Principal;
					children.Add (112);
					break;
				case 112:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "I won't see you acting like a baby anymore.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Bir daha böyle bebek gibi davrandığını görmeyeceğim.";
					}
					character = DialogueCharacter.Principal;
					children.Add (113);
					break;
				case 113:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Go to your room and think about what you've done.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Odana git ve yaptığının kötülüğünü düşün.";
					}
					character = DialogueCharacter.Principal;
					break;
				case 114:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Hey kid.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Selam çocuk.";
					}
					character = DialogueCharacter.Ghost;
					break;
				case 115:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Oh my poor boy! Let's try it again, shall we?";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Ah benim zavallı çocuğum! Hadi bir daha deneyelim, olur mu?";
					}
					character = DialogueCharacter.Nurse;
					children.Add (116);
					break;
				case 116:
					type = DialogueNodeType.QuestionNodeWithQuestion;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "Don't worry, the pill is still clean.";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "Merak etme, hap hala temiz.";
					}
					character = DialogueCharacter.Nurse;
					break;
				/*
				case :
					type = DialogueNodeType.;
					if (GameManagerScript.lang == Language.Eng)
					{
						text = "";
					}
					else if (GameManagerScript.lang == Language.Tur)
					{
						text = "";
					}
					character = DialogueCharacter.;
					break;*/
				default:
					break;
			}

			return new DialogueNode(id, type, text, children, character);
		}

		public static void specialEffect(int id)//called after after answerNode's or questionNodeWithQuestions's
		{
			switch (id)
			{
				case 37:
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(parameterDialogueCharacter, 6);
					break;
				case 50:
					ArcadeFinishingHandler.Create(1, ArcadeGameManager.createNewArcadeGame(GameArcadeKind.SwallowPill));
					break;
				case 78:
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(parameterDialogueCharacter, 9);
					break;
				case 93:
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(parameterDialogueCharacter, 11);
					ArcadeGameManager.createNewArcadeGame(GameArcadeKind.Crying);
					StaticObjectActivator.ActivateInstance();
					break;
				case 113:
					DialogueStarter.FindDialogueStarterAndChangeDialogueId(parameterDialogueCharacter, 12);
					break;
				case 116:
					ArcadeFinishingHandler.Create(1, ArcadeGameManager.createNewArcadeGame(GameArcadeKind.SwallowPill));
					break;
				/*case :

					break;*/
				default:
					break;
			}
		}
	}
}
