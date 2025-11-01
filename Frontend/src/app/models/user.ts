export interface User {
  id: number;
  username: string;
  token?: string;
}

export interface UserWithRoles extends User {
  roles: string[];
}

export interface UserSearchParams {
  page?: number;
  pageSize?: number;
  username?: string;
}
