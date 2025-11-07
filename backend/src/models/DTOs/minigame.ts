export interface MinigameDto {
  id: string;
  name: string;
  iconId?: string;
}
export interface GetGamesResponse {
  games?: MinigameDto[];
}
