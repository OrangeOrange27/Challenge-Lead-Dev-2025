export interface LoginRequest {
  deviceId: string;
}

export interface LoginResponse {
  token?: string;
  playerId?: string;
}