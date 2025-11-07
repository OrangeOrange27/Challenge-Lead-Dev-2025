export interface LoginRequest {
  deviceId: string;
}

export interface LoginResponse {
  success: boolean;
  token?: string;
  playerId?: string;
  error?: string;
}