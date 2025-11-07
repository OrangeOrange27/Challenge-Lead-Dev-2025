export interface ResponseModel<T> {
    success: boolean;
    data?: T;
    error?: string;
}