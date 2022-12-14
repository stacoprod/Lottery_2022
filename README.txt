INSTALLATION MYLOTERIE.COMM: (Version Beta)
###########################################

1) LANCEMENT DES TIRAGES AUTOMATIQUES:
--------------------------------------

1) recherchez "Planificateur de tâche" dans la barre de recherche windows
   Ouvrez l'utilitaire

2) Dans l'onglet "action" => choisissez "importer une tâche"
   Sélectionnez le fichier: Tâche Lancement automatique Tirages.XML
   (Il se trouve dans le dossier principal de Lottery_2022, dans le même dossier que ce fichier-ci)

3) Vous retrouverez la tâche fraîchement importée dans la colonne de gauche, en déployant "Bibliothèque du Planificateur"
   Dans la liste des tâches vérifiez bien que le statut est à "Prêt"

   IMPORTANT: vérifiez dans l'onglet "Actions" , puis Modifier, que le chemin d'accès du programme pointe bien vers:
   "votre repository git"/Lottery_2022/ DrawManager/published/DrawManager.exe

4) Pour désactiver la tâche, et donc suspendre les tirages, faites clic droit sur la tâche et "Désactiver"
   ("Activer" pour la relancer si besoin)

2) LANCEMENT DE L'APPLICATION WEB:
----------------------------------

1) Ouvrez Visual Studio et choisissez le bon projet à ouvrir dans votre repository git

2) Lancez la solution Lottery_2022, et attendez que la solution compile

3) Le navigateur se lance: ça y est! Vous pouvez enfin jouer à la loterie et perdre de l'argent!

Bon jeu!
########
