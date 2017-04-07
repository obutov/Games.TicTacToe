(function () {
    'use strict';

    angular
        .module('app')
        .controller('Main', main);

    function main($scope, $http) {

        var game = this;

        game.moveDelay = 500;
        game.messageDelay = 2000;
        game.paused = true;
        game.ended = false;
        game.gameBoardHidden = true;

        game.player1 = null;
        game.player1Score = 0;
        game.player2 = null;
        game.player2Score = 0;

        game.cellsArray = [];

        game.alertClass = "alert-info";
        game.message = "Welcome";
        game.matchOpponents = "vs.";


        $http.get("TournamentResults/TournamentResults.json").then(function (res) {
            game.results = res.data;
            game.initGame();
        });

        game.initGame = function () {
            game.roundNum = 0;
            game.bracketNum = 0;
            game.matchNum = 0;
            game.moveNum = 0;

            game.boardSize = game.results.BoardSize;
            game.numOfRounds = game.results.NumberOfRounds;

            game.round = game.results.Rounds[game.roundNum];

            game.brackets = game.round.Brackets;

            game.resizeBracketsPanel();
            $(".board-panel").width(game.boardSize * 120);

            game.boardArraySize = game.boardSize * game.boardSize;
            game.cellsArray = game.initBoardArray(game.boardArraySize, " ");
        }

        game.start = function () {
            if (game.paused) {
                game.gameBoardHidden = false;
                game.paused = false;
                setTimeout(function () {
                   game.playTournament();
                }, 0);
            }
            else {
                game.paused = true;
            }
        };

        game.initBoardArray = function (size, symbol) {
            var arr = [];
            for (var i = 0; i < size; i++) {
                arr[i] = symbol;
            }
            return arr;
        };

        game.playTournament = function () {
            game.moveInterval = setInterval(function () {
                game.applyMove();
                $scope.$digest();
            }, game.moveDelay);
        }

        game.applyMove = function () {
            // stop the game when no moves left
            if (game.ended) {
                game.paused;
                clearInterval(game.moveInterval);
                $("#startButton").prop("disabled", true);
                game.alertClass = "alert-success";
                game.message = game.results.WinnerName != "" ? "Congratulations! " + game.results.WinnerName + " is the winner of the tournament." : "There was a tie in the last round.";
                return;
            }
            if (!game.paused) {
                // get round
                var round = game.results.Rounds[game.roundNum];

                if (round) {
                    // get bracket
                    game.brackets = round.Brackets;
                    game.resizeBracketsPanel();
                    var bracket = game.brackets[game.bracketNum];

                    if (bracket) {

                        // get match
                        var match = bracket.Matches[game.matchNum];

                        if (match) {
                            // reset board for the new match
                            if (game.moveNum == 0) {                            
                                game.cellsArray = game.initBoardArray(game.boardArraySize, " ");
                            }
                            // display match players
                            game.player1 = match.Player1Name;
                            game.player2 = match.Player2Name;
                            game.matchOpponents = game.player1 + " [ X ] vs. " + game.player2 + " [ O ]";

                            // get move
                            var move = match.MoveHistory[game.moveNum];
                            if (move) {
                                game.playMove(move, game.moveNum % 2 == 0 ? "X" : "O");
                                game.moveNum++;
                            } else {
                                // no more moves in the current match, display results
                                if (match.WinningPlayerName == "") {
                                    game.alertClass = "alert-warning";
                                    game.message = "Match " + game.matchNum + ": DRAW";
                                } else {
                                    game.alertClass = "alert-success";
                                    game.message = "Match " + game.matchNum + ": " + match.WinningPlayerName + " WON";
                                }

                                // record match results
                                // match players alternate, so need to check for matching name
                                bracket.Player1Score = bracket.Player1Name == match.Player1Name ? match.Player1Score : match.Player2Score;
                                bracket.Player2Score = bracket.Player2Name == match.Player2Name ? match.Player2Score : match.Player1Score;

                                // reset moveNum count and proceed to the the next match
                                game.moveNum = 0;
                                game.matchNum++;
                            }
                        } else {
                            // no more matches in the current bracket, display results
                            // test if this is the only bracket in the round
                            if (game.brackets.length != 1) {

                                if (bracket.Player1Score == bracket.Player2Score) {
                                    setTimeout(function () {
                                        game.alertClass = "alert-warning";
                                        game.message = "Bracket " + game.bracketNum + ": Both players are advancing to the next round. (Click RESUME to continue)";
                                        $scope.$digest();
                                    }, game.messageDelay);

                                } else if (bracket.Player1Score > bracket.Player2Score) {
                                    setTimeout(function () {
                                        game.alertClass = "alert-success";
                                        game.message = "Bracket " + game.bracketNum + ": " + bracket.Player1Name + " is advancing to the next round. (Click RESUME to continue)";
                                        $scope.$digest();
                                    }, game.messageDelay);
                                } else {
                                    setTimeout(function () {
                                        game.alertClass = "alert-success";
                                        game.message = "Bracket " + game.bracketNum + ": " + bracket.Player2Name + " is advancing to the next round. (Click RESUME to continue)";
                                        $scope.$digest();
                                    }, game.messageDelay);
                                }
                                game.matchNum = 0;
                                game.bracketNum++;
                                game.paused = true;
                            // only one bracket in the round indicated LAST round - GAME OVER
                            } else {
                                game.ended = true;
                            }
                        }
                    } else {
                        game.bracketNum = 0;
                        game.roundNum++;
                    }
                } else {
                    game.ended = true;
                }

            } else {
                // pause the game
                clearInterval(game.moveInterval);
            }
        };

        game.playMove = function (move, player) {
            var row = move.Item1;
            var column = move.Item2;

            var idx = row + column * game.boardSize;

            game.cellsArray[idx] = player;
        };

        game.resizeBracketsPanel = function () {
            $(".brackets-panel").width(game.brackets.length * 260);
        };
    }    

})();