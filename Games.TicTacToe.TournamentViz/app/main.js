(function () {
    'use strict';

    angular
        .module('app')
        .controller('Main', main);

    function main($http) {

        var vz = this;

        vz.paused = true;
        vz.gameBoardHidden = true;
        vz.roundNumber = 0;
        vz.player1 = null;
        vz.player1Score = 0;
        vz.player2 = null;
        vz.player2Score = 0;
        vz.cellsArray = [];
        vz.alertClass = "alert-info";
        vz.message = "Welcome";
        vz.matchOpponents = "vs.";


        $http.get("TournamentResults/TournamentResults.json").then(function (res) {
            vz.results = res.data;

            vz.boardSize = vz.results.BoardSize;
            vz.numOfRounds = vz.results.NumberOfRounds;

            vz.round = vz.results.Rounds[vz.roundNumber];
            vz.matchNum = 0;
            vz.brackets = vz.round.Brackets;
            $(".brackets-panel").width(vz.brackets.length * 230);
            $(".board-panel").width(vz.boardSize * 120);
            vz.boardArraySize = vz.boardSize * vz.boardSize;
            vz.cellsArray = vz.initBoardArray(vz.boardArraySize, " ");
        });


        vz.buttonText = "START";

        vz.start = function () {
            if (vz.buttonText == "START") {
                vz.buttonText = "PAUSE";
                vz.gameBoardHidden = false;
                vz.paused = false;
                setTimeout(function () {
                    vz.playTournament();
                }, 0);
            }
            else {
                vz.buttonText = "START";
                vz.paused = true;
            }
        }

        vz.initBoardArray = function (size, symbol) {
            var arr = [];
            for (var i = 0; i < size; i++) {
                arr[i] = symbol;
            }
            return arr;
        }

        vz.playTournament = function () {
            while (!vz.paused && vz.roundNumber < vz.numOfRounds) {
                // get round
                vz.round = vz.results.Rounds[vz.roundNumber];
                vz.message = "Starting Round " + vz.roundNumber;
                setTimeout(function () {
                    var brackets = vz.round.Brackets;

                    // play bracket
                    for (var i = 0; i < brackets.length; i++) {
                        setTimeout(function () {
                            vz.playBracket(brackets[i]);
                        }, 500);
                    }
                    vz.roundNumber++;
                }, 3000);
            }
        }

        vz.playBracket = function (bracket) {
            var matches = bracket.Matches;
            setTimeout(function () {
                for (var i = 0; i < matches.length; i++) {
                    var winningPlayer = vz.playMatch(matches[i]);

                    if (bracket.Player1Name == winningPlayer) {
                        bracket.Player1Score += 1;
                    } else if (bracket.Player2Name == winningPlayer) {
                        bracket.Player2Score += 1;
                    }
                }
                if (bracket.Player1Score == bracket.Player2Score) {
                    vz.message = "Both players are advancing to another round.";
                }
            }, 1000);
        }

        vz.playMatch = function (match) {

            vz.player1 = match.Player1Name;
            vz.player2 = match.Player2Name
            vz.matchOpponents = vz.player1 + " vs. " + vz.player2;
            var moveHistory = match.MoveHistory;

            for (var i = 0; i < moveHistory.length; i++) {
                vz.playMove(moveHistory[i], i % 2 == 0 ? "X" : "O");
            }

            if (match.WinningPlayerName == "") {
                vz.message = "DRAW";
            } else {
                vz.message = match.WinningPlayerName + " WON";
            }
            return match.WinningPlayerName;
        };

        vz.playMove = function (move, player) {
            var row = move.Item1;
            var column = move.Item2;

            var idx = row + column * vz.boardSize;

            vz.cellsArray[idx] = player;
        };
    }    

})();