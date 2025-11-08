import {GameMode} from "../entities";

export interface MinigameDto {
  id: string;
  name: string;
  iconId?: string;
  modes: GameMode[];
}
export interface GetGamesResponse {
  games?: MinigameDto[];
}
