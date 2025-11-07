export interface MinigameDto {
  id: string;
  name: string;
  iconId?: string;
}
export interface GetGamesResponse {
  success: boolean;
  games?: MinigameDto[];
  error?: string;
}
