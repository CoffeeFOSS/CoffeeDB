export interface User {
  id: number;
  username: string;
  token?: string;
}

export interface UserWithRoles extends User {
  roles: string[];
}
