export interface ChangeUsernamePayload {
  newUsername: string;
  password: string;
}

export interface ChangePasswordPayload {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface SignInPayload {
  username: string;
  password: string;
}

export interface RegisterPayload extends SignInPayload {
  confirmPassword: string;
}
