# BiblioGest - Système de Gestion de Bibliothèque

BiblioGest est une application de bureau développée en WPF (.NET) qui permet la gestion complète d'une bibliothèque municipale ou universitaire.

## Description du projet

BiblioGest est conçue pour simplifier le travail quotidien des bibliothécaires en offrant une interface intuitive et complète pour gérer tous les aspects d'une bibliothèque moderne. L'application permet de suivre le catalogue de livres, les inscriptions d'adhérents, les emprunts et retours, ainsi que de générer des statistiques utiles à la gestion.

## Fonctionnalités principales

- **Gestion des livres** : Ajout, modification et suppression des livres avec tous leurs détails (ISBN, titre, auteur, éditeur, année de publication, catégorie)
- **Gestion des adhérents** : Inscription de nouveaux membres, mise à jour des informations personnelles
- **Gestion des emprunts** : Enregistrement des emprunts, suivi des retours et gestion des retards
- **Gestion des catégories** : Organisation des livres par catégories pour faciliter les recherches
- **Tableau de bord** : Vue d'ensemble des statistiques importantes (livres disponibles, emprunts en cours, retards)
- **Recherche intuitive** : Recherche rapide de livres ou d'adhérents selon différents critères
- **Administration** : Gestion des comptes utilisateurs avec différents niveaux d'accès (administrateur et bibliothécaire)

## Technologies utilisées

BiblioGest est développée avec les technologies Microsoft modernes :
- **WPF** (Windows Presentation Foundation) pour l'interface graphique
- **Architecture MVVM** (Model-View-ViewModel) pour une séparation claire des responsabilités
- **Entity Framework Core** pour l'accès aux données et leur persistance
- **SQLite** comme Base de donnee

## Interface utilisateur

L'application propose une interface claire et efficace composée de plusieurs modules :
- Écran de connexion sécurisé
- Tableau de bord avec statistiques et accès rapide aux fonctions principales
- Modules de gestion des livres, adhérents, emprunts et catégories
- Interface d'administration des utilisateurs (accessible uniquement aux administrateurs)

BiblioGest est le résultat d'un projet académique visant à mettre en pratique les connaissances en développement d'applications .NET avec le pattern MVVM et Entity Framework.
