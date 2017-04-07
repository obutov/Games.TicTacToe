(function () {
    'use strict';

    angular
        .module('app')
        .controller('Main', main);

    function main($scope, $http) {

        var game = this;

        $("#confetti").height(0);

        game.moveDelay = 500;
        game.messageDelay = game.moveDelay * 2;
        game.paused = true;
        game.ended = false;
        game.gameBoardHidden = true;
        game.pausingEnabled = true;

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

            $(".slider").slider().on("slide", function (ev) {
                game.moveDelay = ev.value;
            });
        }

        game.startClick = function () {
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

        game.playTournament = function () {
            // apply move at moveDelay interval
            game.moveInterval = setInterval(function () {
                game.applyMove();

                // when in setInterval, need to digest changes
                $scope.$digest();
            }, game.moveDelay);
        }

        game.applyMove = function () {
            // stop the game when no moves left
            if (game.ended) {
                game.paused = true;

                clearInterval(game.moveInterval);

                $("#startButton").prop("disabled", true);
                game.alertClass = "alert-success";
                game.message = game.results.WinnerName != "" ? "Congratulations! " + game.results.WinnerName + " is the winner of the tournament." : "There was a tie in the last round.";
                if (game.brackets[0].Player1Name == game.results.WinnerName)
                    $("." + game.brackets[0].Player2Name).addClass("player-strike-out");
                else
                    $("." + game.brackets[0].Player1Name).addClass("player-strike-out");

                game.celebrate();
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
                        // delay message refresh
                        if (game.moveNum > (1000 / game.moveDelay) * 3) {
                            game.alertClass = "alert-info";
                            game.message = "Bracket " + game.bracketNum + " is in progress.";
                        };
                        // get match
                        var match = bracket.Matches[game.matchNum];

                        if (match) {
                            // reset board for the new match
                            if (game.moveNum == 0) {                            
                                game.cellsArray = game.initBoardArray(game.boardArraySize, " ");
                            }
                            // display match players
                            game.displayMatchPlayers(match);

                            // get move
                            var move = match.MoveHistory[game.moveNum];

                            if (move) {
                                game.playMove(move, game.moveNum % 2 == 0 ? "X" : "O");
                                game.moveNum++;
                            } else {
                                // if there are no more moves, display match results
                                game.displayMatchResults(match, bracket);

                                // reset move counter
                                game.moveNum = 0;

                                // advance to the the next match
                                game.matchNum++;
                            }
                        } else {
                            // no more matches in the current bracket, display results
                            // test if there more than 1 brackets in the current round
                            if (game.brackets.length != 1) {

                                game.displayBracketResults(bracket);

                                //reset move counter
                                game.matchNum = 0;

                                // advance to then next bracket
                                game.bracketNum++;

                                // pause game, if enabled
                                if (game.pausingEnabled)
                                    game.paused = true;
                            // only one bracket in the round indicats LAST round - GAME OVER
                            } else {
                                game.ended = true;
                            }
                        }
                    } else {
                        // no more brackets
                        // reset bracket counter
                        game.bracketNum = 0;
                        // advance to the next round
                        game.roundNum++;
                    }
                } else {
                    // no more rounds
                    // game ended
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

        game.displayBracketResults = function (bracket) {
            if (bracket.Player1Score == bracket.Player2Score) {
                setTimeout(function () {
                    game.alertClass = "alert-warning";
                    game.message = "Bracket " + game.bracketNum + ": Both players are advancing to the next round. (Click RESUME to continue)";
                    $scope.$digest();
                }, game.messageDelay);

            } else if (bracket.Player1Score > bracket.Player2Score) {
                $("." + bracket.Player2Name).addClass("player-strike-out");
                setTimeout(function () {
                    game.alertClass = "alert-success";
                    game.message = "Bracket " + game.bracketNum + ": " + bracket.Player1Name + " is advancing to the next round. (Click RESUME to continue)";
                    $scope.$digest();
                }, game.messageDelay);
            } else {
                $("." + bracket.Player1Name).addClass("player-strike-out");
                setTimeout(function () {
                    game.alertClass = "alert-success";
                    game.message = "Bracket " + game.bracketNum + ": " + bracket.Player2Name + " is advancing to the next round. (Click RESUME to continue)";
                    $scope.$digest();
                }, game.messageDelay);
            }
        };

        game.displayMatchPlayers = function(match){
            game.player1 = match.Player1Name;
            game.player2 = match.Player2Name;
            game.matchOpponents = game.player1 + " [ X ] vs. " + game.player2 + " [ O ]";
        }

        game.displayMatchResults = function (match, bracket) {
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
        };

        game.resizeBracketsPanel = function () {
            $(".brackets-panel").width(game.brackets.length * 260);
        };

        game.initBoardArray = function (size, symbol) {
            var arr = [];
            for (var i = 0; i < size; i++) {
                arr[i] = symbol;
            }
            return arr;
        };
    }    

})();